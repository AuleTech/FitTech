using Bogus;
using FitTech.Application.Configuration;

namespace FitTech.UnitTests.Data.Generators;

public sealed class AuthenticationSettingsFakeGenerator : Faker<AuthenticationSettings>
{
    public AuthenticationSettingsFakeGenerator()
    {
        RuleFor(x => x.Audience, faker => faker.Random.String());
        RuleFor(x => x.SigningKey, faker => faker.Random.String());
        RuleFor(x => x.Issuer, faker => faker.Random.String());
    }
}
