using System.Web;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Domain.Aggregates.AuthAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Auth.ResetPassword;

public interface IResetPasswordCommandHandler : IAuleTechCommandHandler<ResetPasswordCommand, Result>;

internal sealed class ResetPasswordCommandHandler : IResetPasswordCommandHandler
{
    private readonly ILogger<ResetPasswordCommandHandler> _logger;
    private readonly UserManager<FitTechUser> _userManager;

    public ResetPasswordCommandHandler(UserManager<FitTechUser> userManager,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }
        
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
