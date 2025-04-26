using Bogus;
using FitTech.Application.Auth.Configuration;

namespace FitTech.Api.Tests.Data.Generators;

public sealed class AuthenticationSettingsTestGenerator: Faker<AuthenticationSettings>
{
    public AuthenticationSettingsTestGenerator()
    {
        RuleFor(x => x.Audience, faker => faker.Random.String());
        RuleFor(x => x.SigningKey, faker => faker.Random.String());
        RuleFor(x => x.Issuer, faker => faker.Random.String());
    }
}
