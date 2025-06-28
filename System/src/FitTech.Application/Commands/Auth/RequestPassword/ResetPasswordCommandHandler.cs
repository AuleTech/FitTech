using System.Web;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Auth.RequestPassword;

internal sealed class ResetPasswordCommandHandler : IAuleTechCommandHandler<ResetPasswordCommand, Result>
{
    private readonly UserManager<FitTechUser> _userManager;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(UserManager<FitTechUser> userManager, ILogger<ResetPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        //TODO: DtoValidation
        var user = await _userManager.FindByEmailAsync(command.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogError("User not found('{Email}')", command.Email);
            return Result.Failure("Something went wrong");
        }

        var result = await _userManager.ResetPasswordAsync(user, HttpUtility.HtmlDecode(command.Token),
            command.Password);

        return result.ToResult();
    }
}
