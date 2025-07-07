using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Entities;

namespace FitTech.Domain.Repositories;

public interface IClientRepository
{
    Task<Result<Client>> AddAsync(Client client, CancellationToken cancellationToken);
    Task<Result<Client>> GetAsync(Guid id, CancellationToken cancellationToken);
}
