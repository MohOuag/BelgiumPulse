using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BelgiumPulse.Infrastructure.Messaging.Messages;

namespace BelgiumPulse.Infrastructure.Messaging.Consumers;

public class StibDisruptionConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<StibDisruptionConsumer> _logger;
    private IChannel? _channel;
    private const string ExchangeName = "belgaimpulse.events";
    private const string QueueName = "stib.disruptions";
    private const string RoutingKey = "stib.disruption.*";

    public StibDisruptionConsumer(
        IConnection connection,
        ILogger<StibDisruptionConsumer> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _connection.CreateChannelAsync(
            cancellationToken: stoppingToken);

        // Déclare l'exchange
        await _channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: stoppingToken);

        // Déclare la queue
        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true, //survit au redémarrage
            exclusive: false, //plusieurs instances du Consumer peuvent écouter la même queue
            autoDelete: false, // les messages s'accumulent jusqu'au retour du consumer, pas de suppression
            cancellationToken: stoppingToken);

        // Lie la queue à l'exchange via le routing key
        await _channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey,
            cancellationToken: stoppingToken);

        // Configure le consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                //Déserialise le message
                var body = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer
                    .Deserialize<StibDisruptionMessage>(body);

                if (message is null) return;

                _logger.LogInformation(
                    "Perturbation reçue — Ligne {LineNumber}: {Message}",
                    message.LineNumber,
                    message.DisruptionMessage);

                // Ici on notifiera via SignalR dans l'étape suivante
                // Pour l'instant on log simplement

                // Acknowledge — confirme que le message a été traité
                await _channel.BasicAckAsync(
                    args.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur traitement message perturbation STIB");

                // Nack — remet le message dans la queue
                await _channel.BasicNackAsync(
                    args.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false, // on gère l'Ack manuellement - RabbitMQ garde le message jusqu'à ce qu'on lui dise
            consumer: consumer,
            cancellationToken: stoppingToken);

        // Maintient le consumer actif
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}