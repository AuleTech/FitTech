using AuleTech.Core.Messaging;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Commands.Auth.Register;
using FitTech.Application.Commands.Client.Add.Events;
using FitTech.Application.Commands.Trainer.Add.Events;
using FitTech.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FitTech.Infrastructure.Consumers;

internal sealed class TrainerAddedEventConsumer : IAuleTechConsumer<TrainerAddedEvent>
{
    private readonly IAuleTechCommandHandler<RegisterCommand, Result> _commandHandler;
    private readonly ITrainerRepository _trainerRepository;
    private readonly ILogger<TrainerAddedEventConsumer> _logger;
    
    public TrainerAddedEventConsumer(IAuleTechCommandHandler<RegisterCommand, Result> commandHandler, ILogger<TrainerAddedEventConsumer> logger, ITrainerRepository trainerRepository)
    {
        _commandHandler = commandHandler;
        _logger = logger;
        _trainerRepository = trainerRepository;
    }

    public async Task HandleAsync(TrainerAddedEvent? message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering client('{Id}')", message!.Id);
        var result = await _trainerRepository.GetAsync(message.Id, cancellationToken);

        var registerUserResult = await _commandHandler.HandleAsync(new RegisterCommand(result!.Email, "TemporalPassword1", UserType.Trainer),
            cancellationToken);

        if (!registerUserResult.Succeeded)
        {
            throw new Exception(registerUserResult.Errors.First());
        }
    }
}
