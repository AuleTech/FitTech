using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;

namespace FitTech.Persistence.Repositories;

public sealed class ClientRepository : IClientRepository
    {
        private readonly FitTechDbContext _context;

        public ClientRepository(FitTechDbContext context)
        {
            _context = context;
        }
    
        public async Task ClientAsync(Client client, CancellationToken cancellationToken)
        {
            await _context.ClientTable.AddAsync(client, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
