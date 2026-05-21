using System.Text;
using System.Text.Json;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Turbo.Module.Identity.Domain.Events;

namespace Turbo.Module.Identity.Infrastructure.EmailWorker;

public class EmailConsumerWorker(
    IConfiguration config,
    ILogger<EmailConsumerWorker> logger
) : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost",
            UserName = config["RabbitMQ:Username"] ?? "guest",
            Password = config["RabbitMQ:Password"] ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: "user_registered",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<UserRegisteredEvent>(body);

                if (evt is not null)
                    await SendWelcomeEmailAsync(evt, stoppingToken);

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Email sending failed.");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "user_registered",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task SendWelcomeEmailAsync(
        UserRegisteredEvent evt, CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(config["Email:From"]));
        message.To.Add(MailboxAddress.Parse(evt.Email));
        message.Subject = "Xoş gəldiniz!";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <h2>Salam, {evt.FirstName}!</h2>
                <p>Qeydiyyatınız uğurla tamamlandı.</p>
                <p>Email: <strong>{evt.Email}</strong></p>
                """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            config["Email:SmtpHost"],
            int.Parse(config["Email:SmtpPort"] ?? "587"),
            SecureSocketOptions.StartTls, ct);

        await client.AuthenticateAsync(
            config["Email:Username"], config["Email:Password"], ct);

        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("Welcome email sent to {Email}", evt.Email);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}