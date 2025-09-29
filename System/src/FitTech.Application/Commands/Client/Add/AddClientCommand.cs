using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;

namespace FitTech.Application.Commands.Client.Add;

public class AddClientCommand : ICommand, IValidator
{
    public string Name { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public DateTimeOffset Birthdate { get; init; }
    public int TrainingHours { get; init; }
    public string TrainingModel { get; init; } = null!;
    public DateTimeOffset EventDate { get; init; } 
    public string Center { get; init; } = null!;
    public string SubscriptionType { get; init; } = null!;
    public string Email { get; set; } = null!;
    
    public Guid TrainerId { get; init; }
    
    public Result Validate()
    {
        var errors = new List<string>(); //TODO: DateTime.IsOlderThan(Age)

        Email.ValidateStringNullOrEmpty(errors, nameof(Email));
        Name.ValidateStringNullOrEmpty(errors, nameof(Name));
        LastName.ValidateStringNullOrEmpty(errors, nameof(LastName));
        TrainingModel.ValidateStringNullOrEmpty(errors, nameof(TrainingModel));
        Center.ValidateStringNullOrEmpty(errors, nameof(Center));
        SubscriptionType.ValidateStringNullOrEmpty(errors, nameof(SubscriptionType));
        TrainingHours.ValidateGenericMember(() => TrainingHours < 0, errors, nameof(TrainingHours));
        TrainerId.ValidateGenericMember(() => Guid.Empty == TrainerId, errors, nameof(TrainerId));


        return errors.Any() ? Result.Failure(errors.ToArray()) : Result.Success;
    }
}

public static class AddClientCommandExtensions
{
    public static Domain.Entities.Client ToEntity(this AddClientCommand command) => new Domain.Entities.Client
    {
        Name = command.Name,
        LastName = command.LastName,
        Birthdate = command.Birthdate,
        TrainingHours = command.TrainingHours,
        TrainingModel = command.TrainingModel,
        EventDate = command.EventDate,
        Center = command.Center,
        SubscriptionType = command.SubscriptionType,
        TrainerId = command.TrainerId,
        Email = command.Email
    };
} 
