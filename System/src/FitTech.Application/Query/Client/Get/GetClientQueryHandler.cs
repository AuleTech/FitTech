using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Repositories;


namespace FitTech.Application.Query.Client.Get
{
    internal sealed class GetClientDataQueryHandler : IListQueryHandler<GetClientDataQuery, ClientDataDto>
    {
        private readonly IClientRepository _clientRepository;

        public GetClientDataQueryHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        public async Task<Result<List<ClientDataDto>>> HandleGroupAsync(GetClientDataQuery query, CancellationToken cancellationToken)
        {
            var clientResult = await _clientRepository.GetClientsAsync(query.Id, cancellationToken);

            if (!clientResult.Succeeded || clientResult.Value is null || !clientResult.Value.Any())
            {
                return Result<List<ClientDataDto>>.Failure("Clients not found");
            }

            var dtoList = clientResult.Value
                .Select(client => new ClientDataDto(
                    client.Name,
                    client.LastName,
                    client.Email,
                    client.Birthdate,
                    client.TrainingHours,
                    client.TrainingModel,
                    client.EventDate,
                    client.Center,
                    client.SubscriptionType
                ))
                .ToList();

            return Result<List<ClientDataDto>>.Success(dtoList);
        }
    }
}
