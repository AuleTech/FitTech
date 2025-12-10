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

namespace FitTech.Application.Commands.Trainer.ResendInvitations;

public interface IResendInvitationsCommandHandler : IAuleTechCommandHandler<ResendInvitationsCommand, Result>; 

internal class ResendInvitationsCommandHandler : IResendInvitationsCommandHandler
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelInvitationsCommandHandler> _logger;
    private readonly IEmailService _emailService;
    
    public ResendInvitationsCommandHandler(ITrainerRepository trainerRepository, IUnitOfWork unitOfWork, ILogger<CancelInvitationsCommandHandler> logger, IEmailService emailService)
    {
        _trainerRepository = trainerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<Result> HandleAsync(ResendInvitationsCommand command, CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var trainer = await _trainerRepository.GetAsync(command.TrainerId, cancellationToken);

        if (trainer is null)
        {
            _logger.LogError("Trainer('{TrainerId}') not found", command.TrainerId);
            throw new UnauthorizedAccessException("Trainer not found");
        }

        var invitationResult = trainer.CheckInvitation(command.ClientEmail);

        if (!invitationResult.Succeeded)
        {
            return invitationResult;
        }
        
        var invitationCanceled = await _trainerRepository.ResendInvitationAsync(command.ClientEmail, cancellationToken);
        
        await _emailService.SendEmailAsync(command.ClientEmail,
            RegisterClientEmailTemplate.Create(invitationResult.Value!.Code, trainer.Name), cancellationToken);
        
        await _unitOfWork.SaveAsync(cancellationToken);
        
        _logger.LogDebug("Invitation('{InvitationId}') have been resended to {Email}", invitationCanceled.Email, invitationCanceled.UpdatedUtc);
        
        return Result.Success;
    }
}
