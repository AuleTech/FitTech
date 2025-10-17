using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Abstranctions.Dtos;
using FitTech.Domain.Enums;
using FitTech.Domain.Seedwork;
using FitTech.Domain.ValueObjects;

namespace FitTech.Domain.Aggregates.ClientAggregate;

public class Client : Entity, IAggregateRoot
{
    private Client()
    {
    }

    public Guid TrainerId { get; set; }
    public Guid? SettingsId { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public Address Address { get; set; } = null!;

    public virtual ClientSettings Settings { get; set; } = null!;
    public virtual List<BodyMeasurement> BodyMeasurements { get; set; } = [];

    //TODO: Too many params
    public static Result<Client> Create(Guid trainerId, PersonInfoDto information, CredentialsDto credentials, AddressDto address)
    {
        var errors = new List<string>();
        trainerId.ValidateNotEmpty(errors, nameof(trainerId));
        credentials.Email.ValidateEmail(errors, nameof(credentials.Email));
        information.Name.ValidateStringNullOrEmpty(errors, nameof(information.Name));
        information.LastName.ValidateStringNullOrEmpty(errors, nameof(information.LastName));
        address.City.ValidateStringNullOrEmpty(errors, nameof(address.City));
        address.Country.ValidateStringNullOrEmpty(errors, nameof(address.Country));
        information.PhoneNumber.ValidateStringNullOrEmpty(errors, nameof(information.PhoneNumber));
        information.BirthDate.ValidateIsAdult(errors);

        if (errors.Any())
        {
            return Result<Client>.Failure(errors);
        }

        var client = new Client
        {
            Id = Guid.CreateVersion7(),
            TrainerId = trainerId,
            Email = credentials.Email,
            Name = information.Name,
            LastName = information.LastName,
            Address = new Address { City = address.City, Country = address.Country },
            BirthDate = information.BirthDate,
            PhoneNumber = information.PhoneNumber
        };

        return client;
    }

    public Result CreateSettings(int trainingDaysPerWeek, Guid[] favouriteExercises,
        TrainingGoal goal)
    {
        if (SettingsId is not null)
        {
            return Result.Failure("Settings already exists, use update instead");
        }

        var result = ClientSettings.Create(Id, trainingDaysPerWeek, favouriteExercises, goal);

        if (!result.Succeeded)
        {
            return result;
        }

        SettingsId = result.Value!.Id;
        Settings = result.Value;

        return Result.Success;
    }

    public Result<BodyMeasurement> AddBodyMeasurement(BodyMeasurementDto bodyMeasurement)
    {
        var bodyMeasurementResult =
            BodyMeasurement.Create(Id, bodyMeasurement);

        if (!bodyMeasurementResult.Succeeded)
        {
            return bodyMeasurementResult;
        }

        BodyMeasurements.Add(bodyMeasurementResult.Value!);

        return bodyMeasurementResult;
    }
}
