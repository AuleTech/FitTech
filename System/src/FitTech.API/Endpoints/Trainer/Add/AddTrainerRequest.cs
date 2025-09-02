using FitTech.Application.Commands.Client.Add;
using FitTech.Application.Commands.Trainer.Add.Events;

namespace FitTech.API.Endpoints.Trainer.Add;

public record AddTrainerRequest(
    Guid TrainerId,
    string FirstName,
    string LastName,
    string Email,
    string Password);

public static class AddClientRequestExtensions
{
    public static AddTrainerCommand ToCommand(this AddTrainerRequest request) => new AddTrainerCommand
    {
        TrainerId = request.TrainerId,
        Name = request.FirstName,
        LastName = request.LastName,
        Password = request.Password,
        Email = request.Email
    };
}
