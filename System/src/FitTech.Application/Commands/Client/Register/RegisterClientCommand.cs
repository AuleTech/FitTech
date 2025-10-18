using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Abstranctions.Dtos;
using FitTech.Application.Extensions;

namespace FitTech.Application.Commands.Client.Register;

public record RegisterClientCommand(Guid InvitationId ,PersonInfoDto Information, CredentialsDto Credentials, AddressDto Address, TrainingSettingsDto TrainingSettings, BodyMeasurementDto BodyMeasurement) : ICommand , IValidator
{
    public Result Validate()
    {
        var errors = new List<string>();
        ValidateInformation();
        ValidateCredentials();
        ValidateAddress();
        
        return errors.ToResult();
        
        void ValidateInformation()
        {
            Information.Name.ValidateStringNullOrEmpty(errors, nameof(Information.Name));
            Information.LastName.ValidateStringNullOrEmpty(errors, nameof(Information.LastName));
            Information.PhoneNumber.ValidateStringNullOrEmpty(errors, nameof(Information.PhoneNumber));
        }

        void ValidateCredentials()
        {
            Credentials.Email.ValidateEmail(errors, nameof(Credentials.Email));
            Credentials.Password.ValidateStringNullOrEmpty(errors, nameof(Credentials.Password));
        }

        void ValidateAddress()
        {
            Address.City.ValidateStringNullOrEmpty(errors, nameof(Address.City));
            Address.Country.ValidateStringNullOrEmpty(errors, nameof(Address.Country));
        }
    }
}
