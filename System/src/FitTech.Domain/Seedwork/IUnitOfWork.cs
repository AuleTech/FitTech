namespace FitTech.Domain.Seedwork;

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken cancellationToken);
}
