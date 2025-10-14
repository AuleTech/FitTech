using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;

namespace FitTech.Application.Commands.Trainer.Register;

public record RegisterTrainerCommand(string Name, string LastName, string Email, string Password) : ICommand, IValidator
{
    public Result Validate()
    {
        var errors = new List<string>(); //TODO: DateTime.IsOlderThan(Age)

        Email.ValidateEmail(errors, nameof(Email));
        Name.ValidateStringNullOrEmpty(errors, nameof(Name));
        LastName.ValidateStringNullOrEmpty(errors, nameof(LastName));
        Password.ValidateStringNullOrEmpty(errors, nameof(Password));

        return errors.ToResult();
    }
}
