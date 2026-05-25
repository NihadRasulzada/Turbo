using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Events;
using Turbo.Module.Identity.Infrastructure.Options;

namespace Turbo.Module.Identity.Infrastructure.Messaging;

/// <summary>
/// Singleton publisher — thread safety üçün SemaphoreSlim ilə qorunur.
/// </summary>
public sealed class RabbitMqEventPublisher(
    IOptions<RabbitMqOptions> options
) : IEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _opts = options.Value;

    // Singleton-da eyni anda bir neçə async PublishAsync çağrısı gələ bilər.
    // SemaphoreSlim(1,1) EnsureConnectedAsync içindəki yenidən-qoşulma
    // məntiqini race condition-dan qoruyur.
    private readonly SemaphoreSlim _connectLock = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    private async Task EnsureConnectedAsync(CancellationToken ct = default)
    {
        // Sürətli yol: kanal açıqdırsa kilid əldə etmə
        if (_channel is { IsOpen: true }) return;

        await _connectLock.WaitAsync(ct);
        try
        {
            // İkiqat yoxlama: başqa bir thread artıq qoşulmuş ola bilər
            if (_channel is { IsOpen: true }) return;

            // Köhnə resursları təmizlə
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            var factory = new ConnectionFactory
            {
                HostName = _opts.Host,
                Port = _opts.Port,
                UserName = _opts.Username,
                Password = _opts.Password
            };

            _connection = await factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        }
        finally
        {
            _connectLock.Release();
        }
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : DomainEvent
    {
        await EnsureConnectedAsync(cancellationToken);

        // "UserRegisteredEvent" → "user_registered"
        var eventType = typeof(T).Name.Replace("Event", string.Empty);
        var queue = string.Concat(
            eventType.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())
        ).ToLowerInvariant();

        await _channel!.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        var props = new BasicProperties { Persistent = true };

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
        _connectLock.Dispose();
    }
}
