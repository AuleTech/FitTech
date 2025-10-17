using FitTech.Abstranctions.Dtos;

namespace FitTech.API.Endpoints.Client.Register;

public record RegisterClientRequest(PersonInfoDto Information, CredentialsDto Credentials, AddressDto Address, TrainingSettingsDto TrainingSettings, BodyMeasurementDto BodyMeasurement);
