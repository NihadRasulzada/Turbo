using System.Text;
using System.Text.Json;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Turbo.Module.Identity.Domain.Events;
using Turbo.Module.Identity.Infrastructure.Options;

namespace Turbo.Module.Identity.Infrastructure.EmailWorker;

public sealed class EmailConsumerWorker(
    IOptions<RabbitMqOptions> rabbitOptions,
    IOptions<EmailOptions> emailOptions,
    ILogger<EmailConsumerWorker> logger
) : BackgroundService
{
    private readonly RabbitMqOptions _rabbit = rabbitOptions.Value;
    private readonly EmailOptions _email = emailOptions.Value;

    private const string QueueName = "user_registered";
    private const int MaxConnectionAttempts = 10;

    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // RabbitMQ hazır olmasa (Docker Compose race) retry ilə gözlə
        var factory = new ConnectionFactory
        {
            HostName = _rabbit.Host,
            Port = _rabbit.Port,
            UserName = _rabbit.Username,
            Password = _rabbit.Password
        };

        for (int attempt = 1; attempt <= MaxConnectionAttempts; attempt++)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync(stoppingToken);
                break;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromSeconds(Math.Min(30, attempt * 3));
                logger.LogWarning(ex,
                    "RabbitMQ bağlantı cəhdi {Attempt}/{Max} uğursuz oldu. {Delay}s gözlənilir.",
                    attempt, MaxConnectionAttempts, delay.TotalSeconds);
                await Task.Delay(delay, stoppingToken);
            }
        }

        if (_connection is null)
        {
            logger.LogCritical(
                "RabbitMQ-ya {Max} cəhddən sonra qoşulmaq mümkün olmadı. " +
                "Email worker işə başlamayacaq.", MaxConnectionAttempts);
            return;
        }

        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
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

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Xoş gəldin emaili göndərilə bilmədi. Mesaj rədd edilir (requeue: false).");
                // requeue: false — poison message-ı sonsuz loop-a salmaq əvəzinə at;
                // DLX konfiqurasiyası varsa ora yönləndiriləcək.
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task SendWelcomeEmailAsync(UserRegisteredEvent evt, CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_email.From));
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
        await client.ConnectAsync(_email.SmtpHost, _email.SmtpPort, SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_email.Username, _email.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("Xoş gəldin emaili göndərildi: {Email}", evt.Email);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
