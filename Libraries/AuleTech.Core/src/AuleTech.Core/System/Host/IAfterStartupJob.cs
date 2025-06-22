namespace AuleTech.Core.System.Host;

public interface IAfterStartupJob
{
    Task RunAsync(CancellationToken cancellationToken);
}
