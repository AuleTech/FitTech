namespace AuleTech.Core.Messaging;

public interface IAuleTechConsumer<TMessage>
{
    Task HandleAsync(TMessage? message, CancellationToken cancellationToken);
}
