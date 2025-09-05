
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Repositories;
using FitTech.Persistence.Repositories;


namespace FitTech.Application.Query.Client.Get;

internal sealed class GetClientDataQueryHandler : IQueryHandler<GetClientDataQuery, Result<ClientDataDto>>
{
    
    private readonly IClientRepository _clientRepository;
    public GetClientDataQueryHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
    }

    public async Task<Result<ClientDataDto>> HandleAsync(GetClientDataQuery query, CancellationToken cancellationToken)
    {
        var client  = await _clientRepository.GetAsync(query.Id , cancellationToken);

        if (client is null)
        {
            return Result<ClientDataDto>.Failure("Clients not found");
        }
        
        return new ClientDataDto(client.Value!.Name, client.Value!.LastName, client.Value!.Email, client.Value!.Birthdate, client.Value!.TrainingHours, client.Value!.TrainingModel, client.Value!.EventDate, client.Value!.Center, client.Value!.SubscriptionType);
    }
}
