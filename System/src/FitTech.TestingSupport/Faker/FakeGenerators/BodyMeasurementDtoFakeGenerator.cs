using Bogus;
using FitTech.Abstractions.Dtos;

namespace FitTech.TestingSupport.Faker.FakeGenerators;

public class BodyMeasurementDtoFakeGenerator : RecordFaker<BodyMeasurementDto>
{
    public BodyMeasurementDtoFakeGenerator()
    {
        RuleFor(x => x.Biceps, f => f.Random.Number(5, 100));
        RuleFor(x => x.Chest, f => f.Random.Number(5, 300));
        RuleFor(x => x.Height, f => f.Random.Number(5, 300));
        RuleFor(x => x.Hip, f => f.Random.Number(5, 300));
        RuleFor(x => x.MaxThigh, f => f.Random.Number(5, 300));
        RuleFor(x => x.XShoulders, f => f.Random.Number(5, 300));
    }
}
