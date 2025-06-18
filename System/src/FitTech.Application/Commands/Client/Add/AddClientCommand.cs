using AuleTech.Core.Patterns.CQRS;

namespace FitTech.Application.Commands.Client.Add;

public class AddClientCommand : ICommand
{
    public string Name { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public DateTime Birthdate { get; init; }
    public int TrainingHours { get; init; }
    public string TrainingModel { get; init; } = null!;
    public DateTime EventDate { get; init; } 
    public string Center { get; init; } = null!;
    public string SubscriptionType { get; init; } = null!;
}
