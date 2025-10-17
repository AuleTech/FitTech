using FitTech.Domain.Aggregates.ClientAggregate;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Repositories;

public interface IClientRepository : IRepository<Client>
{
    Task AddAsync(Client client, CancellationToken cancellationToken);
}
