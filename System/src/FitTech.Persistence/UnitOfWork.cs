using FitTech.Domain.Seedwork;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitTech.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly FitTechDbContext _context;

    public UnitOfWork(FitTechDbContext context)
    {
        _context = context;
    }

    public async Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
