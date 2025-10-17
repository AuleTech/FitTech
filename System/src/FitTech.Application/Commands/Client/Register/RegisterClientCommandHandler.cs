using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Services;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<FitTechUser> _userManager;

    public RegisterClientCommandHandler(IClientRepository clientRepository, IUnitOfWork unitOfWork, UserManager<FitTechUser> userManager, ILogger<RegisterClientCommandHandler> logger) 
        : base(unitOfWork,logger)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    protected override async Task<Result> HandleTransactionAsync(RegisterClientCommand command, CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var createClientResult = Domain.Aggregates.ClientAggregate.Client.Create(command.TrainerId, command.Information, command.Credentials, command.Address);

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
        
        return Result.Success;
    }
}
