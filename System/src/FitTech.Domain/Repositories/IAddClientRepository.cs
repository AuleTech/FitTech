using FitTech.Domain.Entities;

namespace FitTech.Domain.Repositories;

public interface IClientRepository
{
    Task ClientAsync(Client client, CancellationToken cancellationToken);
}
