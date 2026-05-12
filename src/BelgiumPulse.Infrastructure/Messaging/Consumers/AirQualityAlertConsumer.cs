using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BelgiumPulse.Infrastructure.Messaging.Messages;

namespace BelgiumPulse.Infrastructure.Messaging.Consumers;

public class AirQualityAlertConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<AirQualityAlertConsumer> _logger;
    private IChannel? _channel;
    private const string ExchangeName = "belgaimpulse.events";
    private const string QueueName = "airquality.alerts";
    private const string RoutingKey = "airquality.alert.*";

    public AirQualityAlertConsumer(
        IConnection connection,
        ILogger<AirQualityAlertConsumer> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _connection.CreateChannelAsync(
            cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer
                    .Deserialize<AirQualityAlertMessage>(body);

                if (message is null) return;

                _logger.LogInformation(
                    "Alerte qualité air reçue — Station {Station}: AQI {Aqi} ({Level})",
                    message.StationName,
                    message.AqiValue,
                    message.AqiLevel);

                await _channel.BasicAckAsync(
                    args.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur traitement message qualité de l'air");

                await _channel.BasicNackAsync(
                    args.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}