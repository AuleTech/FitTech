using Bogus;
using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Domain.Enums;
using FitTech.TestingSupport;
using FitTech.TestingSupport.Faker;

namespace FitTech.UnitTests.Data.Generators;

public sealed class InvitationFakeGenerator : FakerWithPrivateConstructor<Invitation>
{
    public InvitationFakeGenerator()
    {
        RuleFor(x => x.Email, f => f.Person.Email);
        RuleFor(x => x.Id, _ => Guid.NewGuid());
        RuleFor(x => x.CreatedUtc, f => f.Date.Between(DateTime.UtcNow.AddDays(-16), DateTime.UtcNow));
        RuleFor(x => x.Status, f => f.Random.Enum<InvitationStatus>());
    }

    public InvitationFakeGenerator WithEmail(string email)
    {
        RuleForOverride(x => x.Email, f => email);

        return this;
    }
    
    public InvitationFakeGenerator WithStatus(InvitationStatus invitationStatus)
    {
        RuleForOverride(x => x.Status, f => invitationStatus);

        return this;
    }
}
