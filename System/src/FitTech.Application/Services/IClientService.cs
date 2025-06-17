using AuleTech.Core.Patterns;
using FitTech.Application.Dtos.Client;
using FitTech.Domain.Entities;

namespace FitTech.Application.Services;

public interface IClientService
{
    Task<Result<ClientSettingsDto>> GetSettingsAsync(Guid id, CancellationToken cancellationToken);

    Task<Result> AddAsync(AddClientDto client, CancellationToken cancellationToken);
}
