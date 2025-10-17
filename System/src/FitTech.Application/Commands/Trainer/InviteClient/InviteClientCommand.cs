using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;

namespace FitTech.Application.Commands.Trainer.InviteClient;

public record InviteClientCommand(Guid TrainerId, string ClientEmail) : ICommand, IValidator
{
    public Result Validate()
    {
        var errors = new List<string>();
        
        TrainerId.ValidateNotEmpty(errors, nameof(TrainerId));
        ClientEmail.ValidateEmail(errors, nameof(ClientEmail));

        return errors.ToResult();
    }
}
