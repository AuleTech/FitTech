using System.Text;
using System.Text.Json;
using AuleTech.Core.Messaging.Rabbit.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AuleTech.Core.Messaging.Rabbit;

public class AuleTechRabbitQueueListener<TMessage> : IAuleTechQueueListener, IAsyncDisposable
{
    private readonly RabbitMqConfiguration _configuration;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IEnumerable<IAuleTechConsumer<TMessage>> _consumers;
    private readonly ILogger<AuleTechRabbitQueueListener<TMessage>> _logger;
    private IChannel? _channel;
    private IConnection? _connection;
    private IChannel? _dlqChannel;


    public AuleTechRabbitQueueListener(IEnumerable<IAuleTechConsumer<TMessage>> consumers,
        RabbitMqConfiguration configuration, ILogger<AuleTechRabbitQueueListener<TMessage>> logger,
        IConnectionFactory connectionFactory)
    {
        _consumers = consumers;
        _configuration = configuration;
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_dlqChannel is not null)
        {
            await _dlqChannel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Initializing Channel");
        _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        _dlqChannel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await ConsumeQueueAsync();
        await ConsumeDlQueueAsync();

        async Task ConsumeQueueAsync()
        {
            await _channel.QueueDeclareAsync(RabbitExtensions.GetQueueName<TMessage>(), true, false, false,
                cancellationToken: cancellationToken);

            var rabbitConsumer = new AsyncEventingBasicConsumer(_channel);
            rabbitConsumer.ReceivedAsync += HandleReceivedAsync;
            await _channel.BasicConsumeAsync(RabbitExtensions.GetQueueName<TMessage>(), false, rabbitConsumer,
                cancellationToken);

            async Task HandleReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
            {
                var body = eventArgs.Body.ToArray();
                var auleTechMessage = JsonSerializer.Deserialize<AuleTechMessage<TMessage>>(body);

                if (auleTechMessage is null)
                {
                    _logger.LogError("The current message couldn't be consumed, will be discarded");
                    _logger.LogDebug("Discarded message: {Message}", Encoding.UTF8.GetString(body));
                    await _channel.BasicRejectAsync(eventArgs.DeliveryTag, false, cancellationToken);
                }

                var failed = new List<string>();

                foreach (var consumer in _consumers)
                {
                    try
                    {
                        await consumer.HandleAsync(auleTechMessage!.Message, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        failed.Add(ex.Message);
                    }
                }

                auleTechMessage!.Errors = failed.ToArray();

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
            }
        }

        async Task ConsumeDlQueueAsync()
        {
            await _dlqChannel!.QueueDeclareAsync(RabbitExtensions.GetDlQueueName<TMessage>(), true, false, false,
                cancellationToken: cancellationToken);

            var rabbitConsumer = new AsyncEventingBasicConsumer(_dlqChannel);
            rabbitConsumer.ReceivedAsync += HandleDlqReceivedAsync;
            await _channel.BasicConsumeAsync(RabbitExtensions.GetDlQueueName<TMessage>(), false, rabbitConsumer,
                cancellationToken);

            async Task HandleDlqReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
            {
                var body = eventArgs.Body.ToArray();

                var auleTechMessage = JsonSerializer.Deserialize<AuleTechMessage<TMessage>>(body);

                if (auleTechMessage is null)
                {
                    _logger.LogError("The current message couldn't be consumed, will be discarded");
                    _logger.LogDebug("Discarded message: {Message}", Encoding.UTF8.GetString(body));
                    await _dlqChannel.BasicRejectAsync(eventArgs.DeliveryTag, false, cancellationToken);
                }

                var failed = new List<string>();

                foreach (var consumer in _consumers)
                {
                    try
                    {
                        await consumer.HandleAsync(auleTechMessage!.Message, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        failed.Add($"Consumer('{consumer.GetType().Name}') failed handling message: {ex.Message}");
                    }
                }

                auleTechMessage!.Errors = failed.ToArray();

                await _dlqChannel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);

                if (!auleTechMessage.Succeeded)
                {
                    if (auleTechMessage.RetriesCount > _configuration.MaxRetries)
                    {
                        _logger.LogError("Maximum retries exceeded for message('{Id}')", auleTechMessage.Id);
                    }
                }
            }
        }
    }

    ~AuleTechRabbitQueueListener()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }
}
