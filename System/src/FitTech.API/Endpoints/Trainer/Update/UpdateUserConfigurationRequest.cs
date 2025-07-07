using FitTech.Application.Commands.Client.Add;
using FitTech.Application.Commands.Trainer.Update;

namespace FitTech.API.Endpoints.Trainer.Update;

public record UpdateUSerConfigurationRequest(
    string Name,
    string Email,
    string Password
    );

public static class UpdateUSerConfigurationRequestExtensions
{
    public static UpdateTrainerCommand ToCommand(this UpdateUSerConfigurationRequest request) => new UpdateTrainerCommand()
    {
        Name = request.Name,
        Email = request.Email,
        Password = request.Password,
    
    };
}
