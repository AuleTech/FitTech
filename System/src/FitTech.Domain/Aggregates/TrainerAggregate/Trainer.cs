using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Aggregates.TrainerAggregate;

public class Trainer : Entity, IAggregateRoot
{
    internal Trainer()
    {
    }

    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;

    public virtual List<Client> Clients { get; set; } = [];
    public virtual List<Invitation> Invitations { get; set; } = [];

    public static Result<Trainer> Create(string name, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Trainer>.Failure($"{nameof(Name)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<Trainer>.Failure($"{nameof(LastName)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<Trainer>.Failure($"{nameof(Email)} cannot be empty");
        }

        return new Trainer
        {
            Name = name, LastName = lastName, Email = email.ToLowerInvariant(), Id = Guid.CreateVersion7()
        };
    }

    public Result<Invitation> InviteClient(string clientEmail)
    {
        var invitationResult = Invitation.Create(Id, clientEmail);

        if (!invitationResult.Succeeded)
        {
            return invitationResult;
        }
        
        Invitations.Add(invitationResult.Value!);
        
        return Result<Invitation>.Success(invitationResult.Value!);
    }
}
