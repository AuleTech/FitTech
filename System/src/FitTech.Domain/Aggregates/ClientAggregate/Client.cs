using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Abstractions.Dtos;
using FitTech.Domain.Enums;
using FitTech.Domain.Seedwork;
using FitTech.Domain.ValueObjects;

namespace FitTech.Domain.Aggregates.ClientAggregate;

public class Client : Entity, IAggregateRoot
{
    internal Client()
    {
    }

    public Guid TrainerId { get; private set; }
    public Guid? SettingsId { get; private set; }
    public string Email { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }
    public Address Address { get; private set; } = null!;

    public virtual ClientSettings Settings { get; set; } = null!;
    public virtual List<BodyMeasurement> BodyMeasurements { get; set; } = [];

    //TODO: Too many params
    public static Result<Client> Create(Guid trainerId, PersonInfoDto information, CredentialsDto credentials)
    {
        var errors = new List<string>();
        trainerId.ValidateNotEmpty(errors, nameof(trainerId));
        credentials.Email.ValidateEmail(errors, nameof(credentials.Email));
        information.Name.ValidateStringNullOrEmpty(errors, nameof(information.Name));
        information.LastName.ValidateStringNullOrEmpty(errors, nameof(information.LastName));
        information.Address.City.ValidateStringNullOrEmpty(errors, nameof(information.Address.City));
        information.Address.Country.ValidateStringNullOrEmpty(errors, nameof(information.Address.Country));
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
            Address = new Address { City = information.Address.City, Country = information.Address.Country },
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
