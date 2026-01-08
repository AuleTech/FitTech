using Bogus;
using FitTech.Abstractions.Dtos;

namespace FitTech.TestingSupport.Faker.FakeGenerators;

public class CredentialsDtoFakeGenerator : RecordFaker<CredentialsDto>
{
    public CredentialsDtoFakeGenerator()
    {
        RuleFor(x => x.Email, _ => FitTechEmailTestExtensions.GetTestEmail());
        RuleFor(x => x.Password, _ => FitTechTestingSupport.GetTestValidPassword());
    }
}
