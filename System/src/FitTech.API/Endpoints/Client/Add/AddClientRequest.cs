using FitTech.Application.Commands.Client.Add;

namespace FitTech.API.Endpoints.Client.Add;

public record AddClientRequest(string Name,
    string LastName,
    string EmailUser,
    DateTime Birthdate,
    int PhoneNumber,
    int TrainingHours,
    string TrainingModel,
    DateTime EventDate,
    string Center,
    string SubscriptionType);

public static class AddClientRequestExtensions
{
    public static AddClientCommand ToCommand(this AddClientRequest request) => new AddClientCommand
    {
        Name = request.Name,
        LastName = request.LastName,
        Birthdate = request.Birthdate,
        TrainingHours = request.TrainingHours,
        TrainingModel = request.TrainingModel,
        EventDate = request.EventDate,
        Center = request.Center,
        SubscriptionType = request.SubscriptionType
    };
}
