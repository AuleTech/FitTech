using FitTech.Domain.Seedwork;

namespace FitTech.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly FitTechDbContext _context;

    public UnitOfWork(FitTechDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
