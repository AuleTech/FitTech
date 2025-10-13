using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AuleTech.Core.Messaging.Rabbit;

internal sealed class RabbitQueuePublisher : IAuleTechQueuePublisher
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitQueuePublisher> _logger;

    public RabbitQueuePublisher(IConnectionFactory connectionFactory, ILogger<RabbitQueuePublisher> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(RabbitExtensions.GetQueueName<T>(), true, false, false,
            cancellationToken: cancellationToken);

        var auleTechMessage = AuleTechMessage<T>.Create(message);

        await channel.BasicPublishAsync(string.Empty, RabbitExtensions.GetQueueName<T>(),
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(auleTechMessage)), cancellationToken);

        _logger.LogInformation("Message('{Id}') of type {Type} queued", auleTechMessage.Id,
            auleTechMessage.Message!.GetType().Name);
    }

    public async Task PublishDlqAsync<T>(AuleTechMessage<T> message, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(RabbitExtensions.GetDlQueueName<T>(), true, false, false,
            cancellationToken: cancellationToken);

        message.Retry();

        await channel.BasicPublishAsync(string.Empty, message.Id.ToString(),
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)), cancellationToken);

        _logger.LogInformation("Message('{Id}') of type {Type} sent to dlq.", message.Id,
            message.Message!.GetType().Name);
    }
}
