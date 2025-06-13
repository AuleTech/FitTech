using FitTech.Domain.Entities;

namespace FitTech.Domain.Repositories;

public interface IAddClientRepository
{
    Task AddClientAsync(Client client, CancellationToken cancellationToken);
}
