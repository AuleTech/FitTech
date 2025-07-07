namespace AuleTech.Core.Messaging;

public interface IAuleTechQueueListener
{
    Task StartAsync(CancellationToken cancellationToken);
}
