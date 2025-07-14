namespace AuleTech.Core.Messaging;

public interface IAuleTechQueuePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken);
    internal Task PublishDlqAsync<T>(AuleTechMessage<T> message, CancellationToken cancellationToken);
}
