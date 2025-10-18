using FitTech.Abstranctions.Dtos;

namespace FitTech.API.Endpoints.Client.Register;

public record RegisterClientRequest(Guid InvitationId, PersonInfoDto Information, CredentialsDto Credentials, AddressDto Address, TrainingSettingsDto TrainingSettings, BodyMeasurementDto BodyMeasurement);
