using FitTech.Application.Auth.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Services;

public class NewClientService : INewClientService
{
    private readonly ILogger<Client> _logger;
    private readonly IClientRepository _addClientrepository;
    
    public NewClientService(IClientRepository repository, ILogger<Client> logger)
    {
        _logger = logger;
        _addClientrepository = repository;
    }

    public async Task NewClientAsync(Client client, CancellationToken cancellationToken)
    {
       ArgumentNullException.ThrowIfNull(client);

        await _addClientrepository.ClientAsync(client, cancellationToken);
        _logger.LogInformation("New client added");
    }
}
