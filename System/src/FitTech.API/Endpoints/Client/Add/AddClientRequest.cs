namespace FitTech.API.Endpoints.Client.Add;

public record AddClientRequest(string NameUser,
    string LastNameuser,
    string EmailUser,
    DateTime Birthdate,
    int PhoneNumber,
    int TrainingHours,
    string TrainingModel,
    DateTime EventDate,
    string Center,
    string SubscriptionType);

//TODO: Need mappers
