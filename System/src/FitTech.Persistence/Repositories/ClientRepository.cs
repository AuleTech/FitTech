using FitTech.Domain.Aggregates.ClientAggregate;
using FitTech.Domain.Repositories;

namespace FitTech.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly FitTechDbContext _context;

    public ClientRepository(FitTechDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken) =>
        await _context.AddAsync(client, cancellationToken);
}
