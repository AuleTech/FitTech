using Bogus;
using FitTech.Abstranctions.Dtos;

namespace FitTech.TestingSupport.Faker.FakeGenerators;

public class AddressDtoFakeGenerator : RecordFaker<AddressDto>
{
    public AddressDtoFakeGenerator()
    {
        RuleFor(x => x.City, f => f.Address.City());
        RuleFor(x => x.Country, f => f.Address.Country());
    }
}
