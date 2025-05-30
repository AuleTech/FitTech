namespace FitTech.API.Endpoints.User.AddClient;

public record AddNewClientRequest(string NameUser,
    string LastNameuser,
    string EmailUser,
    DateOnly Birthdate,
    int PhoneNumber,
    int TrainingHours,
    string TrainingModel,
    DateOnly EventDate,
    string Center,
    string SubscriptionType);
