using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Enums;
using FitTech.Domain.Seedwork;
using FitTech.Domain.Services;

namespace FitTech.Domain.Aggregates.TrainerAggregate;

public class Invitation : Entity
{
    public Guid TrainerId { get; set; }
    public string Email { get; set; } = null!;
    public InvitationStatus Status { get; set; }
    public int Code { get; set; }
    
    internal Invitation()
    {
    }

    internal static Result<Invitation> Create(Guid trainerId, string email)
    {
        var errors = new List<string>();
        trainerId.ValidateGenericMember(x => x == Guid.Empty, errors, "TrainerId cannot be empty");
        email.ValidateEmail(errors, nameof(email));

        if (errors.Any())
        {
            return Result<Invitation>.Failure(errors);
        }

        return new Invitation()
        {
            TrainerId = trainerId,
            Email = email,
            Status = InvitationStatus.Pending,
            Code = InvitationCodeGenerator.Instance.Generate()
        };
    }
}
