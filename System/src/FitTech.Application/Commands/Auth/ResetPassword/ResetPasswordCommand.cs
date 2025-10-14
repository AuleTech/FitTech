using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;

namespace FitTech.Application.Commands.Auth.ResetPassword;

public record ResetPasswordCommand(string Email, string Password, string Token) : ICommand, IValidator
{
    public Result Validate()
    {
        var errors = new List<string>();
        Email.ValidateEmail(errors, nameof(Email));
        Password.ValidateStringNullOrEmpty(errors, nameof(Password));
        Token.ValidateStringNullOrEmpty(errors, nameof(Token));

        return errors.ToResult();
    }
}
