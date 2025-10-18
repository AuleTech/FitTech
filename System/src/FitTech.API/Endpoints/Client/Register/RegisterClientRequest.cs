using FitTech.Abstranctions.Dtos;

namespace FitTech.API.Endpoints.Client.Register;

public record RegisterClientRequest(Guid InvitationId, PersonInfoDto Information, CredentialsDto Credentials, TrainingSettingsDto TrainingSettings, BodyMeasurementDto BodyMeasurement);
