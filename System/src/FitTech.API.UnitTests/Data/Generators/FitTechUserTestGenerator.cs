using Bogus;
using FitTech.Domain.Entities;

namespace FitTech.API.UnitTests.Data.Generators;

public sealed class FitTechUserTestGenerator : Faker<FitTechUser>
{
    public FitTechUserTestGenerator()
    {
        RuleFor(x => x.Id, faker => Guid.CreateVersion7());
        RuleFor(x => x.Email, faker => faker.Person.Email);
    }
}
