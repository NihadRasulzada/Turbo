using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Turbo.Module.Identity.Application.Common.Interfaces;

namespace Turbo.Module.Identity.Infrastructure.Messaging;

public class RabbitMqEventPublisher(IConfiguration config) : IEventPublisher, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    private async Task EnsureConnectedAsync()
    {
        if (_channel is { IsOpen: true }) return;

        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(config["RabbitMQ:Port"] ?? "5672"),
            UserName = config["RabbitMQ:Username"] ?? "guest",
            Password = config["RabbitMQ:Password"] ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : class
    {
        await EnsureConnectedAsync();

        var routingKey = typeof(T).Name
            .Replace("Event", string.Empty)
            .ToLower()
            .Insert(4, ".");  // "UserRegistered" → "user.registered"

        var queue = routingKey.Replace(".", "_");

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
