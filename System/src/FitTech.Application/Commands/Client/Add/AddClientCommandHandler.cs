using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Repositories;

namespace FitTech.Application.Commands.Client.Add;

internal sealed class AddClientCommandHandler : IAuleTechCommandHandler<AddClientCommand, Result>
{
    private readonly IClientRepository _clientRepository;

    public AddClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Result> HandleAsync(AddClientCommand command, CancellationToken cancellationToken)
    {
        var result = command.Validate();

        if (!result.Succeeded)
        {
            return result;
        }
        
        return await _clientRepository.AddAsync(command.ToEntity(), cancellationToken);
    }
}
