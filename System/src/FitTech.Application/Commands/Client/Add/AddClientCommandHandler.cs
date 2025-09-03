using AuleTech.Core.Messaging;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Commands.Client.Add.Events;
using FitTech.Domain.Repositories;

namespace FitTech.Application.Commands.Client.Add;

internal sealed class AddClientCommandHandler : IAuleTechCommandHandler<AddClientCommand, Result>
{
    private readonly IClientRepository _clientRepository;
    private readonly IAuleTechQueuePublisher _publisher;

    public AddClientCommandHandler(IClientRepository clientRepository, IAuleTechQueuePublisher publisher)
    {
        _clientRepository = clientRepository;
        _publisher = publisher;
    }

    public async Task<Result> HandleAsync(AddClientCommand command, CancellationToken cancellationToken)
    {
        var result = command.Validate();

        if (!result.Succeeded)
        {
            return result;
        }
        
        var saveResult =  await _clientRepository.AddAsync(command.ToEntity(), cancellationToken);

        if (saveResult.Succeeded)
        {
            await _publisher.PublishAsync(new ClientAddedEvent(saveResult.Value!.Email), cancellationToken);
        }

        return saveResult;
    }
}
