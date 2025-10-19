using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Trainer.ValidateInvitation;

public interface IValidateInvitationCommandHandler : IAuleTechCommandHandler<ValidateInvitationCommand, Result<Guid>>;

internal sealed class ValidateInvitationCommandHandler : TransactionCommandHandler<ValidateInvitationCommand, Result<Guid>>,
    IValidateInvitationCommandHandler
{
    private readonly ITrainerRepository _trainerRepository;

    public ValidateInvitationCommandHandler(ITrainerRepository trainerRepository, IUnitOfWork unitOfWork,
        ILogger<ValidateInvitationCommandHandler> logger) : base(unitOfWork, logger)
    {
        _trainerRepository = trainerRepository;
    }

    protected override async Task<Result<Guid>> HandleTransactionAsync(ValidateInvitationCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult.ToTypedResult<Guid>();
        }

        var trainer = await _trainerRepository.GetAsync(command.TrainerId, cancellationToken);

        var result = trainer!.SetInvitationInProgress(command.Email, command.Code);

        if (!result.Succeeded)
        {
            return result.ToTypedResult<Guid>();
        }
        
        var invitationResult = trainer.GetInvitationByEmailAndCode(command.Email, command.Code);
        
        return invitationResult.Succeeded ? invitationResult.Value!.Id : invitationResult.ToTypedResult<Guid>();
    }
}
