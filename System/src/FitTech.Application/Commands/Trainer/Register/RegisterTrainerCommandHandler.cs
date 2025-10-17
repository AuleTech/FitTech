using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Application.Services;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using FitTech.Domain.Templates.EmailTemplates.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Trainer.Register;

public interface IRegisterTrainerCommandHandler : IAuleTechCommandHandler<RegisterTrainerCommand, Result>;

internal sealed class RegisterTrainerCommandHandler : TransactionCommandHandler<RegisterTrainerCommand, Result>,IRegisterTrainerCommandHandler
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly UserManager<FitTechUser> _userManager;
    private readonly ILogger<RegisterTrainerCommandHandler> _logger;

    public RegisterTrainerCommandHandler(ITrainerRepository trainerRepository, UserManager<FitTechUser> userManager,
        IUnitOfWork unitOfWork, IEmailService emailService, ILogger<RegisterTrainerCommandHandler> logger) : base(unitOfWork, logger)
    {
        _trainerRepository = trainerRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    protected override async Task<Result> HandleTransactionAsync(RegisterTrainerCommand command, CancellationToken cancellationToken)
    {
        var result = command.Validate();

        if (!result.Succeeded)
        {
            return result;
        }

        var createTrainerResult =
            Domain.Aggregates.TrainerAggregate.Trainer.Create(command.Name, command.LastName, command.Email);

        if (!createTrainerResult.Succeeded)
        {
            return createTrainerResult;
        }

        await _trainerRepository.AddAsync(createTrainerResult.Value!, cancellationToken);
        var identityResult = await _userManager.CreateAsync(
                new FitTechUser
                {
                    Id = createTrainerResult.Value!.Id,
                    Email = createTrainerResult.Value!.Email,
                    UserName = createTrainerResult.Value!.Email,
                    NormalizedUserName = createTrainerResult.Value!.Email
                }, command.Password)
            .WaitAsync(cancellationToken);

        if (!identityResult.Succeeded)
        {
            return identityResult.ToResult();
        }

        await _emailService.SendEmailAsync(createTrainerResult.Value!.Email, RegisterTrainerEmailTemplate.Create(),
            cancellationToken);

        return Result.Success;
    }
}
