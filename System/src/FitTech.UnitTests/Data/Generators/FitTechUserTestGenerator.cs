using Bogus;
using FitTech.Domain.Aggregates.AuthAggregate;

namespace FitTech.UnitTests.Data.Generators;

public sealed class FitTechUserTestGenerator : Faker<FitTechUser>
{
    public FitTechUserTestGenerator()
    {
        RuleFor(x => x.Id, faker => Guid.CreateVersion7());
        RuleFor(x => x.Email, faker => faker.Person.Email);
    }
}
