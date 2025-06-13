using FitTech.Application.Auth.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Services;

public class AddClientService : IAddClientService
{
    private readonly ILogger<Client> _logger;
    private readonly IAddClientRepository _addClientrepository;
    
    public AddClientService(IAddClientRepository repository, ILogger<Client> logger)
    {
        _logger = logger;
        _addClientrepository = repository;
    }

    public async Task AddNewClientAsync(Client client, CancellationToken cancellationToken)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        await _addClientrepository.AddClientAsync(client, cancellationToken);
        _logger.LogInformation("New client added");
    }
}
