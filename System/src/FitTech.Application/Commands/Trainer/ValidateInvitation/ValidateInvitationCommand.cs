using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;

namespace FitTech.Application.Commands.Trainer.ValidateInvitation;

public record ValidateInvitationCommand(Guid TrainerId, string Email, int Code) : ICommand, IValidator
{
    public Result Validate()
    {
        var errors = new List<string>();
        Email.ValidateEmail(errors, nameof(Email));

        return errors.ToResult();
    }
}
