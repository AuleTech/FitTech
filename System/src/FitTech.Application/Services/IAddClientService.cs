using FitTech.Domain.Entities;

namespace FitTech.Application.Services;

public interface INewClientService
{
    
    Task NewClientAsync( Client client, CancellationToken cancellationToken);
    
}
