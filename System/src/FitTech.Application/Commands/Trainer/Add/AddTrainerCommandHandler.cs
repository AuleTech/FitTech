

using AuleTech.Core.Messaging;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Commands.Trainer.Add.Events;
using FitTech.Domain.Repositories;

namespace FitTech.Application.Commands.Trainer.Add;

internal sealed class AddTrainerCommandHandler : IAuleTechCommandHandler<AddTrainerCommand, Result>
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IAuleTechQueuePublisher _publisher;

    public AddTrainerCommandHandler(ITrainerRepository trainerRepository, IAuleTechQueuePublisher publisher)
    {
        _trainerRepository = trainerRepository;
        _publisher = publisher;
    }

    public async Task<Result> HandleAsync(AddTrainerCommand command, CancellationToken cancellationToken)
    {
        var result = command.Validate();

        if (!result.Succeeded)
        {
            return result;
        }
        
        var saveResult =  await _trainerRepository.AddAsync(command.ToEntity(), cancellationToken);

        if (saveResult.Succeeded)
        {
            await _publisher.PublishAsync(new TrainerAddedEvent(saveResult.Value!.Id), cancellationToken);
        }

        return saveResult;
    }
}

