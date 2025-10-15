using Bogus;
using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Domain.Enums;

namespace FitTech.UnitTests.Data.Generators;

internal sealed class TrainerTestGenerator : Faker<Trainer>
{
    private readonly InvitationTestGenerator _invitationTestGenerator = new ();
    public TrainerTestGenerator()
    {
        CustomInstantiator(f => (Activator.CreateInstance(typeof(Trainer), nonPublic: true) as Trainer)!);
        RuleFor(x => x.Email, x => x.Person.Email);
        RuleFor(x => x.Id, _ => Guid.CreateVersion7());
        RuleFor(x => x.Name, x => x.Person.FirstName);
        RuleFor(x => x.LastName, x => x.Person.LastName);
    }

    public TrainerTestGenerator WithInvitations()
    {
        _invitationTestGenerator
            .RuleFor(x => x.CreatedUtc, f => f.Date.Between(DateTime.UtcNow.AddDays(-16), DateTime.UtcNow));
        _invitationTestGenerator.RuleFor(x => x.Status, f => f.Random.Enum<InvitationStatus>());

        RuleFor(x => x.Invitations, _ => _invitationTestGenerator.Generate(10));
        
        return this;
    }
}
