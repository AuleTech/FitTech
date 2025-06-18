using System.Web;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Auth.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Templates.EmailsTemplates;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Auth.ForgotPassword;

internal sealed class ForgotPasswordAuleTechCommandHandler : IAuleTechCommandHandler<ForgotPasswordCommand, Result<string>>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordAuleTechCommandHandler> _logger;
    private readonly UserManager<FitTechUser> _userManager;

    public ForgotPasswordAuleTechCommandHandler(ILogger<ForgotPasswordAuleTechCommandHandler> logger,
        UserManager<FitTechUser> userManager, IEmailService emailService)
    {
        _logger = logger;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Result<string>> HandleAsync(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            _logger.LogError("Email is required to recover password");
            return Result<string>.Failure("Email is required to recover password");
        }

        var user = await _userManager.FindByEmailAsync(command.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogError("User not found, returning Ok to avoid getting mail register information");
            return Result<string>.Success(string.Empty);
        }

        var resetPasswordToken =
            await _userManager.GeneratePasswordResetTokenAsync(user)
                .WaitAsync(cancellationToken); //TODO: We need to normalized for http.

        var encodedToken = HttpUtility.UrlEncode(resetPasswordToken);
        var callbackUrl = $"{command.CallbackUrl}?email={command.Email}&token={encodedToken}";

        //TODO: do this async.
        await _emailService.SendEmailAsync(
            command.Email,
            ResetPasswordTemplate.Create(callbackUrl),
            cancellationToken
        );

        return Result<string>.Success(HttpUtility.UrlEncode(resetPasswordToken));
    }
}
