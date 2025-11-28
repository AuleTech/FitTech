using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;

namespace FitTech.Application.Query.Trainer.GetInvitations;

public record GetInvitationsQuery(Guid TrainerId) : IQuery, IValidator
{
    public Result Validate()
    {
        var errors = new List<string>();

        TrainerId.ValidateNotEmpty(errors, nameof(TrainerId));

        return errors.ToResult();
    }
}
