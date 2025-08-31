
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;


namespace FitTech.Application.Commands.Trainer.Add.Events;

public class AddTrainerCommand : ICommand, IValidator
{
    public string Name { get; init; } = null!;
    public string LastName { get; init; } = null!;
    
    public string Email { get; init; } = null!;
    
    public string Password { get; init; } = null!;
   
    public Guid TrainerId { get; set; }
    
    
    public Result Validate()
    {
        var errors = new List<string>(); //TODO: DateTime.IsOlderThan(Age)

        Email.ValidateStringNullOrEmpty(errors, nameof(Email));
        Name.ValidateStringNullOrEmpty(errors, nameof(Name));
        LastName.ValidateStringNullOrEmpty(errors, nameof(LastName));
        Password.ValidateStringNullOrEmpty(errors, nameof(Password));
        TrainerId.ValidateGenericMember(() => Guid.Empty == TrainerId, errors, nameof(TrainerId));

        return errors.Any() ? Result.Failure(errors.ToArray()) : Result.Success;
    }
}

public static class AddClientCommandExtensions
{
    public static Domain.Entities.Trainer ToEntity(this AddTrainerCommand command) => new Domain.Entities.Trainer
    {
        Name = command.Name,
        LastName = command.LastName,
        Email = command.Email,
        Password = command.Password,
        Id = command.TrainerId,
       
    };
} 

