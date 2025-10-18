using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Enums;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Client.Register;

public interface IRegisterClientCommandHandler : IAuleTechCommandHandler<RegisterClientCommand, Result>;

internal class RegisterClientCommandHandler : TransactionCommandHandler<RegisterClientCommand, Result>, IRegisterClientCommandHandler
{
    private readonly IClientRepository _clientRepository;
    private readonly ITrainerRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<FitTechUser> _userManager;

    public RegisterClientCommandHandler(IClientRepository clientRepository, IUnitOfWork unitOfWork, UserManager<FitTechUser> userManager, ILogger<RegisterClientCommandHandler> logger, ITrainerRepository trainerRepository) 
        : base(unitOfWork,logger)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _trainerRepository = trainerRepository;
    }

    protected override async Task<Result> HandleTransactionAsync(RegisterClientCommand command, CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var trainer = await _trainerRepository.GetByInvitationId(command.InvitationId, cancellationToken);

        var canRegisterClient = trainer.CanRegisterClient(command.InvitationId, command.Credentials.Email);

        if (!canRegisterClient.Succeeded)
        {
            return canRegisterClient;
        }
        
        var createClientResult = Domain.Aggregates.ClientAggregate.
            Client.Create(trainer.Id, command.Information, command.Credentials);

        if (!createClientResult.Succeeded)
        {
            return createClientResult;
        }

        var client = createClientResult.Value!;

        var settingsResult = client.CreateSettings(command.TrainingSettings.TotalDaysPerWeek,
            command.TrainingSettings.FavouriteExercises,
            (TrainingGoal)command.TrainingSettings.Goal);

        if (!settingsResult.Succeeded)
        {
            return settingsResult;
        }

        var bodyMeasurementResult = client.AddBodyMeasurement(command.BodyMeasurement);

        if (!bodyMeasurementResult.Succeeded)
        {
            return bodyMeasurementResult;
        }
        
        await _clientRepository.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        
        var identityResult = await _userManager.CreateAsync(
                new FitTechUser
                {
                    Id = client.Id,
                    Email = client.Email.ToLowerInvariant(),
                    UserName = client.Email.ToLowerInvariant(),
                    NormalizedUserName = client.Email.ToLowerInvariant()
                }, command.Credentials.Password)
            .WaitAsync(cancellationToken);

        if (!identityResult.Succeeded)
        {
            return identityResult.ToResult();
        }
        
        //TODO: Move to an event, 2 Aggregate root updated in the same transaction.
        trainer.CompleteClientRegistration(client.Id);
        
        return Result.Success;
    }
}
