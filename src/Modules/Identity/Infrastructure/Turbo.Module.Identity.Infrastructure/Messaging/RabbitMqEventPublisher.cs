using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Events;
using Turbo.Module.Identity.Infrastructure.Options;

namespace Turbo.Module.Identity.Infrastructure.Messaging;

public sealed class RabbitMqEventPublisher(
    IOptions<RabbitMqOptions> options
) : IEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _opts = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    private async Task EnsureConnectedAsync()
    {
        if (_channel is { IsOpen: true }) return;

        var factory = new ConnectionFactory
        {
            HostName = _opts.Host,
            Port = _opts.Port,
            UserName = _opts.Username,
            Password = _opts.Password
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : DomainEvent
    {
        await EnsureConnectedAsync();

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
        if (_channel != null) await _channel.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
    }
}
