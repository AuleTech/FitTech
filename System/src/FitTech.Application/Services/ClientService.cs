using AuleTech.Core.Patterns;
using FitTech.Application.Dtos.Client;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Services;

internal sealed class ClientService : IClientService
{
    private readonly UserManager<FitTechUser> _userManager;

    private readonly IClientRepository _clientRepository;

    public ClientService(UserManager<FitTechUser> userManager, IClientRepository clientRepository)
    {
        _userManager = userManager;
        _clientRepository = clientRepository;
    }

    public async Task<Result<ClientSettingsDto>> GetSettingsAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString()).WaitAsync(cancellationToken);

        if (user is null)
        {
            return Result<ClientSettingsDto>.Failure("User not found");
        }

        return new ClientSettingsDto(user.UserName!, "Trainer name", user.Email!);
    }
    
    public async Task<Result> AddAsync(Client client, CancellationToken cancellationToken)
    {
        return await _clientRepository.AddAsync(client, cancellationToken);
    }
}
