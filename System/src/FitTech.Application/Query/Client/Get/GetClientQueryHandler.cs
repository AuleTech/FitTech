
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Dtos;
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

    public async Task<Result<List<ClientDataDto>>> HandleGrupAsync(GetClientDataQuery query, CancellationToken cancellationToken)
    {
        var clientResult = await _clientRepository.GetAsync(query.Id, cancellationToken);

        if (!clientResult.Succeeded || clientResult.Value is null)
        {
            return Result<List<ClientDataDto>>.Failure("Client not found");
        }
        
        var clients = clientResult.Value;

        var dtoList = clientResult.Value
            .Select(client => new ClientDataDto(
            {
                Name = client.Name,
                LastName = client.LastName,
                Birthdate = client.Birthdate,
                TrainingHours = client.TrainingHours,
                TrainingModel = client.TrainingModel,
                EventDate = client.EventDate,
                Center = client.Center,
                SubscriptionType = client.SubscriptionType,
                
            }).ToList();
        

        return Result<List<ClientDataDto>>.Success(dtoList);
        
    }

}
