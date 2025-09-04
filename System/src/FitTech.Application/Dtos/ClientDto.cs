namespace FitTech.Application.Dtos;

public record ClientDto (string Name,
    string LastName,
    string Email,
    DateTime Birthdate,
    int TrainingHours,
    string TrainingModel,
    DateTime EventDate,
    string Center,
    string SubscriptionType);
