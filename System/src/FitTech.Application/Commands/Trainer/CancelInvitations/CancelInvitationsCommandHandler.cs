using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Commands.Trainer.CancelInvitations;
using FitTech.Application.Extensions;
using FitTech.Application.Services;
using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using FitTech.Domain.Templates.EmailTemplates.Register;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Trainer.CancelInvitations;

public interface ICancelInvitationsCommandHandler : IAuleTechCommandHandler<CancelInvitationsCommand, Result>; 

internal class CancelInvitationsCommandHandler : ICancelInvitationsCommandHandler
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelInvitationsCommandHandler> _logger;
    
    public CancelInvitationsCommandHandler(ITrainerRepository trainerRepository, IUnitOfWork unitOfWork, ILogger<CancelInvitationsCommandHandler> logger)
    {
        _trainerRepository = trainerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CancelInvitationsCommand command, CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }
        
        
        var invitationCanceled = await _trainerRepository.CancelInvitationAsync(command.ClientEmail, cancellationToken);
        
        await _unitOfWork.SaveAsync(cancellationToken);
        
        _logger.LogDebug("Invitation('{InvitationId}') have been canceled to {Email}", invitationCanceled.Email, invitationCanceled.UpdatedUtc);
        
        return Result.Success;
    }
}
