using AuleTech.Core.Patterns;
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
    
        public async Task<Result> AddAsync(Client client, CancellationToken cancellationToken)
        {
            await _context.ClientTable.AddAsync(client, cancellationToken);
            var rows = await _context.SaveChangesAsync(cancellationToken);
            
            return rows < 1 ? Result.Failure("Nothing was saved") : Result.Success;
        }
    }
