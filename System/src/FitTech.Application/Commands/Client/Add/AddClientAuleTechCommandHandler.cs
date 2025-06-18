using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Repositories;

namespace FitTech.Application.Commands.Client.Add;

internal sealed class AddClientAuleTechCommandHandler : IAuleTechCommandHandler<AddClientCommand, Result>
{
    private readonly IClientRepository _clientRepository;

    public AddClientAuleTechCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Result> HandleAsync(AddClientCommand command, CancellationToken cancellationToken)
    {
        //TODO: Need mapper
        //TODO: Missing migrations
        return await _clientRepository.AddAsync(new Domain.Entities.Client(), cancellationToken);
    }
}
