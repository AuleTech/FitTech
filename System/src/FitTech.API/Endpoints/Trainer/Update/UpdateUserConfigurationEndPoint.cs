using System.Security.Claims;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Commands.Client.Add;
using FitTech.Application.Commands.Trainer.Update;


namespace FitTech.API.Endpoints.Trainer.Update;


[HttpPost("/configuration/UpdateUserConfiguration")]

public class UpdateUserConfigurationEndPoint: Endpoint<UpdateUSerConfigurationRequest>
{
    
    private readonly IAuleTechCommandHandler<UpdateTrainerCommand, Result> _commandHandler;
    
    public UpdateUserConfigurationEndPoint(IAuleTechCommandHandler<UpdateTrainerCommand, Result> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(UpdateUSerConfigurationRequest req, CancellationToken ct)
    {
        //TODO: Probably add Id when created or something 
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = req.ToCommand();
        command.Id = Guid.Parse(userId);
        
        var result = await _commandHandler.HandleAsync(req.ToCommand(), ct);

        if (!result.Succeeded)
        {
            ThrowError(result.Errors.First());
        }
        
        await SendOkAsync(null, ct);
    }
}
