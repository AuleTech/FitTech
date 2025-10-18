using Bogus;
using FitTech.Domain.Aggregates.ClientAggregate;
using FitTech.TestingSupport;
using FitTech.TestingSupport.Faker;

namespace FitTech.UnitTests.Data.Generators;

public class ClientFakeGenerator : FakerWithPrivateConstructor<Client>
{
    public ClientFakeGenerator()
    {
        RuleFor(x => x.Email, x => x.Person.Email);
        RuleFor(x => x.Id, _ => Guid.CreateVersion7());
        RuleFor(x => x.Name, x => x.Person.FirstName);
        RuleFor(x => x.LastName, x => x.Person.LastName);
    }

    public ClientFakeGenerator WithEmail(string email)
    {
        RuleForOverride(x => x.Email, f => email);

        return this;
    }
}
