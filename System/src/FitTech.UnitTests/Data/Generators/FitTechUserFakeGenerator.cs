using Bogus;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.TestingSupport;
using FitTech.TestingSupport.Faker;

namespace FitTech.UnitTests.Data.Generators;

public sealed class FitTechUserFakeGenerator : FakerWithPrivateConstructor<FitTechUser>
{
    public FitTechUserFakeGenerator()
    {
        RuleFor(x => x.Id, faker => Guid.CreateVersion7());
        RuleFor(x => x.Email, faker => faker.Person.Email);
    }
}
