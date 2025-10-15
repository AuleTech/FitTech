using Bogus;
using FitTech.Domain.Aggregates.TrainerAggregate;

namespace FitTech.UnitTests.Data.Generators;

public sealed class InvitationTestGenerator : Faker<Invitation>
{
    public InvitationTestGenerator()
    {
        CustomInstantiator(f => (Activator.CreateInstance(typeof(Invitation), nonPublic: true) as Invitation)!);
        RuleFor(x => x.Email, f => f.Person.Email);
        RuleFor(x => x.Id, _ => Guid.NewGuid());
    }
}
