using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Application.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Templates;
using FitTech.Domain.Templates.EmailTemplates.Register;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Commands.Auth.Register;

internal sealed class RegisterCommandHandler : IAuleTechCommandHandler<RegisterCommand, Result>
{
    private readonly UserManager<FitTechUser> _userManager;
    private readonly IEmailService _emailService;
    public RegisterCommandHandler(UserManager<FitTechUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Result> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _userManager.CreateAsync(command.MapToIdentityUser(), command.Password)
            .WaitAsync(cancellationToken);

        IEmailTemplate template = command.UserType is UserType.Client
            ? RegisterClientEmailTemplate.Create(command.Email, command.Password)
            : RegisterTrainerEmailTemplate.Create();

        await _emailService.SendEmailAsync(command.Email, template, cancellationToken);
        
        return result.ToResult();
    }
}
