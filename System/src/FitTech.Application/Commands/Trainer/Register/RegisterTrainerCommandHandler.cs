using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Application.Services;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using FitTech.Domain.Templates.EmailTemplates.Register;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Commands.Trainer.Register;

public interface IRegisterTrainerCommandHandler : IAuleTechCommandHandler<RegisterTrainerCommand, Result>;

internal sealed class RegisterTrainerCommandHandler : IRegisterTrainerCommandHandler
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly UserManager<FitTechUser> _userManager;

    public RegisterTrainerCommandHandler(ITrainerRepository trainerRepository, UserManager<FitTechUser> userManager,
        IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _trainerRepository = trainerRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result> HandleAsync(RegisterTrainerCommand command, CancellationToken cancellationToken)
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

        await _unitOfWork.SaveAsync(cancellationToken);

        await _emailService.SendEmailAsync(createTrainerResult.Value!.Email, RegisterTrainerEmailTemplate.Create(),
            cancellationToken);

        return Result.Success;
    }
}
