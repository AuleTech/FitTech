using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Enums;
using FitTech.Domain.Seedwork;
using FitTech.Domain.Services;

namespace FitTech.Domain.Aggregates.TrainerAggregate;

public class Invitation : Entity
{
    internal Invitation()
    {
    }

    public Guid TrainerId { get; private set; }
    public string Email { get; private set; } = null!;
    public InvitationStatus Status { get; set; }
    public int Code { get; private set; }

    internal static Result<Invitation> Create(Guid trainerId, string email)
    {
        var errors = new List<string>();
        trainerId.ValidateGenericMember(x => x != Guid.Empty, errors, "TrainerId cannot be empty");
        email.ValidateEmail(errors, nameof(email));

        if (errors.Any())
        {
            return Result<Invitation>.Failure(errors);
        }

        return new Invitation
        {
            TrainerId = trainerId,
            Email = email,
            Status = InvitationStatus.Pending,
            Code = InvitationCodeGenerator.Instance.Generate()
        };
    }

    internal Result SetInProgress()
    {
        if (Status is InvitationStatus.Completed or InvitationStatus.Expired)
        {
            return Result.Failure("The invitation is on a terminal status");
        }

        Status = InvitationStatus.InProgress;

        return Result.Success;
    }

    internal Result SetCompleted()
    {
        if (Status is InvitationStatus.Pending or InvitationStatus.Expired)
        {
            return Result.Failure("The invitation should be InProgress to complete it");
        }

        Status = InvitationStatus.Completed;

        return Result.Success;
    }
}
