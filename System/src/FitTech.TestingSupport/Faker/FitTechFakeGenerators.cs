using FitTech.TestingSupport.Faker.FakeGenerators;

namespace FitTech.TestingSupport.Faker;

public class FitTechFakeGenerators
{
    public readonly Lazy<AddressDtoFakeGenerator> AddressDto = new ();
    public readonly Lazy<BodyMeasurementDtoFakeGenerator> BodyMeasurementDto = new();
    public readonly Lazy<CredentialsDtoFakeGenerator> CredentialsDto = new();
    public readonly Lazy<PersonInfoDtoFakeGenerator> PersonInfoDto = new();
    public readonly Lazy<TrainingSettingsDtoFakeGenerator> TrainingSettingsDto = new();
    
    public static FitTechFakeGenerators Create() => new ();
}
