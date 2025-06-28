using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;

namespace FitTech.Persistence.Repositories;

internal sealed class EmailRepository : IEmailRepository
{
    private readonly FitTechDbContext _context;

    public EmailRepository(FitTechDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Email email, CancellationToken cancellationToken)
    {
        await _context.EmailLog.AddAsync(email, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
