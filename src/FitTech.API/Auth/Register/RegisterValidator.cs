using FastEndpoints;
using FluentValidation;

namespace FitTech.API.Auth.Register;

public class RegisterValidator : Validator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty")
            .EmailAddress()
            .WithMessage("Not a valid email address");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty")
            .MinimumLength(8)
            .WithMessage("Password needs to be bigger than 8");
    }
}
