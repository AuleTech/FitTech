
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence.Repositories;

public sealed class ClientRepository : IClientRepository
    {
        private readonly FitTechDbContext _context;

        public ClientRepository(FitTechDbContext context)
        {
            _context = context;
        }
    
        public async Task<Result<Client>> AddAsync(Client client, CancellationToken cancellationToken)
        {
            await _context.Client.AddAsync(client, cancellationToken);
            var rows = await _context.SaveChangesAsync(cancellationToken);
            
            return rows < 1 ? Result<Client>.Failure("Nothing was saved") : Result<Client>.Success(client);
        }

        public async Task<Result<Client>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Client.SingleAsync(x => x.Id == id, cancellationToken);
            
        }

        public async Task<Result<List<Client>>> GetClientsAsync(Guid id, CancellationToken cancellationToken)
        {
            var clients = await _context.Client
                .Where(x => x.TrainerId == id)
                .ToListAsync(cancellationToken);

            return Result<List<Client>>.Success(clients);
        }
    }
