using Bogus;
using FitTech.Abstranctions.Dtos;

namespace FitTech.TestingSupport.Faker.FakeGenerators;

public class TrainingSettingsDtoFakeGenerator : RecordFaker<TrainingSettingsDto>
{
    public TrainingSettingsDtoFakeGenerator()
    {
        RuleFor(x => x.TotalDaysPerWeek, _ => 5);
    }
}
