using FitTech.ApiClient.Generated;

namespace FitTech.Client.Mobile.Pages.Auth.Registration.Models;

public record RegistrationForm(
    ValidationForm ValidationForm,
    CredentialsForm Credentials,
    PersonalInformationForm PersonalInformation,
    BodyMeasuresForm BodyMeasures,
    TrainingPreferencesForm TrainingPreferences);

internal static class RegistrationFormExtensions
{
    public static RegisterClientRequest ToRegisterClientRequest(this RegistrationForm registrationForm) =>
        new RegisterClientRequest
        {
            InvitationId = registrationForm.ValidationForm.InvitationId,
            Information =
                new PersonInfoDto
                {
                    Name = registrationForm.PersonalInformation.Name,
                    LastName = registrationForm.PersonalInformation.LastName,
                    PhoneNumber = registrationForm.PersonalInformation.PhoneNumber,
                    BirthDate = DateTime.Now.AddYears(-20),
                    Address =
                        new AddressDto
                        {
                            City = registrationForm.PersonalInformation.City,
                            Country = registrationForm.PersonalInformation.Country
                        }
                },
            Credentials =
                new CredentialsDto
                {
                    Email = registrationForm.Credentials.Password, Password = registrationForm.Credentials.Password
                },
            TrainingSettings =
                new TrainingSettingsDto
                {
                    TotalDaysPerWeek = registrationForm.TrainingPreferences.AvailableDays!.Value,
                    Goal = TrainingGoalEnumDto.BodyBuilding,
                    FavouriteExercises = []
                },
            BodyMeasurement = new BodyMeasurementDto
            {
                Hip = registrationForm.BodyMeasures.Hip!.Value,
                MaxThigh = registrationForm.BodyMeasures.MaxThigh!.Value,
                Biceps = registrationForm.BodyMeasures.Biceps!.Value,
                XShoulders = registrationForm.BodyMeasures.XShoulders!.Value,
                Chest = registrationForm.BodyMeasures.Chest!.Value,
                Height = registrationForm.BodyMeasures.Height!.Value,
                Weight = registrationForm.BodyMeasures.Weight!.Value
            }
        };
}
