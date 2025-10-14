using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;

namespace FitTech.Application.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand, IValidator
{
    public Result Validate()
    {
        var errors = new List<string>();
        
        Email.ValidateEmail(errors, nameof(Email));
        Password.ValidateStringNullOrEmpty(errors, nameof(Password));

        return errors.ToResult();
    }
}

public record LoginResultDto(string AccessToken, string RefreshToken);
