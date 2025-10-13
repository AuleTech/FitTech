using FitTech.Application.Commands.Trainer.Register;

namespace FitTech.API.Endpoints.Trainer.Register;

public record RegisterTrainerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);

public static class AddClientRequestExtensions
{
    public static RegisterTrainerCommand ToCommand(this RegisterTrainerRequest request) => new (request.FirstName, request.LastName, request.Email, request.Password);
}
