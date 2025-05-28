using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;

namespace FitTech.Persistence.Repositories;

internal sealed class AddClientRepository : IAddClientRepository
    {
        private readonly FitTechDbContext _context;

        public AddClientRepository(FitTechDbContext context)
        {
            _context = context;
        }
    
        public async Task AddClientAsync(Client client, CancellationToken cancellationToken)
        {
            await _context.ClientLog.AddAsync(client, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
