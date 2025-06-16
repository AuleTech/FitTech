using AuleTech.Core.Patterns;
using FitTech.Domain.Entities;

namespace FitTech.Domain.Repositories;

public interface IClientRepository
{
    Task<Result> AddAsync(Client client, CancellationToken cancellationToken);
}
