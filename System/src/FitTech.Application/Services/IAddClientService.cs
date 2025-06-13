using FitTech.Domain.Entities;

namespace FitTech.Application.Services;

public interface IAddClientService
{
    
    Task AddNewClientAsync( Client client, CancellationToken cancellationToken);
    
}
