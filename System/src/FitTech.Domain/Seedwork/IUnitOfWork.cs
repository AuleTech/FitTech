using Microsoft.EntityFrameworkCore.Storage;

namespace FitTech.Domain.Seedwork;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
}
