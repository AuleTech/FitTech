using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Abstractions.Dtos;
using FitTech.Application.Extensions;

namespace FitTech.Application.Commands.Client.Register;

public record RegisterClientCommand(Guid InvitationId ,PersonInfoDto Information, CredentialsDto Credentials, TrainingSettingsDto TrainingSettings, BodyMeasurementDto BodyMeasurement) : ICommand , IValidator
{
    public Result Validate()
    {
        var errors = new List<string>();
        ValidateInformation();
        ValidateCredentials();
        
        return errors.ToResult();
        
        void ValidateInformation()
        {
            Information.Name.ValidateStringNullOrEmpty(errors, nameof(Information.Name));
            Information.LastName.ValidateStringNullOrEmpty(errors, nameof(Information.LastName));
            Information.PhoneNumber.ValidateStringNullOrEmpty(errors, nameof(Information.PhoneNumber));
            ValidateAddress(Information.Address);
            
        }

        void ValidateCredentials()
        {
            Credentials.Email.ValidateStringNullOrEmpty(errors, nameof(Credentials.Email));
            Credentials.Password.ValidateStringNullOrEmpty(errors, nameof(Credentials.Password));
        }

        void ValidateAddress(AddressDto addressDto)
        {
            addressDto.City.ValidateStringNullOrEmpty(errors, nameof(addressDto.City));
            addressDto.Country.ValidateStringNullOrEmpty(errors, nameof(addressDto.Country));
        }
    }
}
