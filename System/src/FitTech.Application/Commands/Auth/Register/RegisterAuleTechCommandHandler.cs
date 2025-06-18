using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Commands.Auth.Register;

internal sealed class RegisterAuleTechCommandHandler : IAuleTechCommandHandler<RegisterCommand, Result>
{
    private readonly UserManager<FitTechUser> _userManager;

    public RegisterAuleTechCommandHandler(UserManager<FitTechUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _userManager.CreateAsync(command.MapToIdentityUser(), command.Password)
            .WaitAsync(cancellationToken);

        return result.ToResult();
    }
}
