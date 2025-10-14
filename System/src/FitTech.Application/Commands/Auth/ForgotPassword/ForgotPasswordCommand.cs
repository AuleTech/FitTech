using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;

namespace FitTech.Application.Commands.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email, string CallbackUrl) : ICommand, IValidator
{
    public Result Validate()
    {
        var errors = new List<string>();
        
        Email.ValidateEmail(errors, nameof(Email));
        CallbackUrl.ValidateUrl(errors, nameof(CallbackUrl));

        return errors.Any() ? Result.Failure(errors) : Result.Success;
    }
}
