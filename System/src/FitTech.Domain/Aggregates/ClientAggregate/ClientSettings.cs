using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Enums;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Aggregates.ClientAggregate;

public class ClientSettings : Entity
{
    public Guid ClientId { get; private set; }
    public int TrainingDaysPerWeek { get; private set; }
    public Guid[] FavouriteExercises { get; private set; } = [];
    public TrainingGoal Goal { get; private set; }

    internal static Result<ClientSettings> Create(Guid clientId, int trainingDaysPerWeek, Guid[] favouriteExercises, TrainingGoal goal)
    {
        var errors = new List<string>();
        
        clientId.ValidateNotEmpty(errors, nameof(clientId));
        trainingDaysPerWeek.ValidateAge(errors, nameof(trainingDaysPerWeek));

        if (errors.Any())
        {
            return Result<ClientSettings>.Failure(errors);
        }

        return new ClientSettings()
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            TrainingDaysPerWeek = trainingDaysPerWeek,
            FavouriteExercises = favouriteExercises,
            Goal = goal
        };
    }
}
