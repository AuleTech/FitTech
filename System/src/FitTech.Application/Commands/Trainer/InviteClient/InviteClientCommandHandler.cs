using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Application.Services;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using FitTech.Domain.Templates.EmailTemplates.Register;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Trainer.InviteClient;

public interface IInviteClientCommandHandler : IAuleTechCommandHandler<InviteClientCommand, Result>; 

internal class InviteClientCommandHandler : IInviteClientCommandHandler
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<InviteClientCommandHandler> _logger;
    
    public InviteClientCommandHandler(ITrainerRepository trainerRepository, IUnitOfWork unitOfWork, IEmailService emailService, ILogger<InviteClientCommandHandler> logger)
    {
        _trainerRepository = trainerRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(InviteClientCommand command, CancellationToken cancellationToken)
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

        var invitationResult = trainer.InviteClient(command.ClientEmail);

        if (!invitationResult.Succeeded)
        {
            return invitationResult;
        }

        await _trainerRepository.AddInvitationAsync(invitationResult.Value!, cancellationToken);

        await _emailService.SendEmailAsync(command.ClientEmail,
            RegisterClientEmailTemplate.Create(invitationResult.Value!.Code, trainer.Name), cancellationToken);
        
        await _unitOfWork.SaveAsync(cancellationToken);
        
        _logger.LogDebug("Invitation('{InvitationId}') sent to {Email}", invitationResult.Value!.Id, invitationResult.Value!.Email);
        
        return Result.Success;
    }
}
