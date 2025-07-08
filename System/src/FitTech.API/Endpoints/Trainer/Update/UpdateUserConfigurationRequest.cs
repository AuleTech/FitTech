using FitTech.Application.Commands.Client.Add;
using FitTech.Application.Commands.Trainer.Update;

namespace FitTech.API.Endpoints.Trainer.Update;

public record UpdateUSerConfigurationRequest(
    Guid Id,
    string Name,
    string Email,
    string Password
    );

public static class UpdateUSerConfigurationRequestExtensions
{
    public static UpdateTrainerCommand ToCommand(this UpdateUSerConfigurationRequest request) => new UpdateTrainerCommand()
    {
        Id = request.Id,
        Name = request.Name,
        Email = request.Email,
        Password = request.Password,
    
    };
}
