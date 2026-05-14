using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Infrastructure.Messaging.Publishers;

public class RabbitMqPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private const string ExchangeName = "belgaimpulse.events";

    public RabbitMqPublisher(
        IConnection connection,
        ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        // Déclare l'exchange au démarrage
        _channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(
        T message,
        string routingKey,
        CancellationToken cancellationToken = default) where T : class
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true, // message survit au redémarrage de RabbitMQ
            ContentType = "application/json",
            Timestamp = new AmqpTimestamp(
                DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Message publié — Exchange: {Exchange}, RoutingKey: {RoutingKey}",
            ExchangeName, routingKey);
    }

    public async ValueTask DisposeAsync()
    {
        //Onferme proprement le cana et la connexion
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}