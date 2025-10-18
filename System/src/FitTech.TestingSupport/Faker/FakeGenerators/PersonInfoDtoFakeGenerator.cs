using Bogus;
using FitTech.Abstranctions.Dtos;

namespace FitTech.TestingSupport.Faker.FakeGenerators;

public class PersonInfoDtoFakeGenerator : RecordFaker<PersonInfoDto>
{
    private readonly AddressDtoFakeGenerator _addressDtoFakeGenerator = new ();
    
    public PersonInfoDtoFakeGenerator()
    {
        RuleFor(x => x.Name, f => f.Person.FirstName);
        RuleFor(x => x.LastName, f => f.Person.LastName);
        RuleFor(x => x.BirthDate, f => DateOnly.FromDateTime(f.Date.Between(DateTime.UtcNow.AddYears(-18), DateTime.UtcNow.AddYears(-50))));
        RuleFor(x => x.PhoneNumber, f => f.Person.Phone);
        RuleFor(x => x.Address, _ => _addressDtoFakeGenerator);
    }
}
