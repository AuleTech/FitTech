namespace FitTech.API.Endpoints.User.AddClient;

public record AddNewClientRequest(string NameUser,
    string LastNameuser,
    string EmailUser,
    DateTime Birthdate,
    int PhoneNumber,
    int TrainingHours,
    string TrainingModel,
    DateTime EventDate,
    string Center,
    string SubscriptionType);
