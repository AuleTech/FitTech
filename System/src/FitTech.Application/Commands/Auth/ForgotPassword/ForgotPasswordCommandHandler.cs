using System.Web;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Application.Services;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Seedwork;
using FitTech.Domain.Templates.EmailTemplates.ResetPassword;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Auth.ForgotPassword;

public interface IForgotPasswordCommandHandler : IAuleTechCommandHandler<ForgotPasswordCommand, Result<string>>;

internal sealed class ForgotPasswordCommandHandler : TransactionCommandHandler<ForgotPasswordCommand, Result<string>>,IForgotPasswordCommandHandler
{
    private readonly IEmailService _emailService;
    private readonly UserManager<FitTechUser> _userManager;

    public ForgotPasswordCommandHandler(ILogger<ForgotPasswordCommandHandler> logger,
        UserManager<FitTechUser> userManager, IEmailService emailService, IUnitOfWork unitOfWork) : base(unitOfWork, logger)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    protected override async Task<Result<string>> HandleTransactionAsync(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult.ToTypedResult<string>();
        }

        var user = await _userManager.FindByEmailAsync(command.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            Logger.LogError("User not found, returning Ok to avoid getting mail register information");
            return Result<string>.Success(string.Empty);
        }

        var resetPasswordToken =
            await _userManager.GeneratePasswordResetTokenAsync(user)
                .WaitAsync(cancellationToken); //TODO: We need to normalized for http.

        var encodedToken = HttpUtility.UrlEncode(resetPasswordToken);
        var callbackUrl = $"{command.CallbackUrl}?email={command.Email}&token={encodedToken}";
        
        await _emailService.SendEmailAsync(
            command.Email,
            ResetPasswordTemplate.Create(callbackUrl),
            cancellationToken
        );
        
        return Result<string>.Success(HttpUtility.UrlEncode(resetPasswordToken));
    }
}
