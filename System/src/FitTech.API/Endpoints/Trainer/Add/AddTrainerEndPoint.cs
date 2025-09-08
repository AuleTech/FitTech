using System.Security.Claims;
using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Commands.Client.Add;
using FitTech.Application.Commands.Trainer.Add.Events;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Trainer.Add;


[HttpPost("/user/add-trainer")]
public class AddTrainerEndPoint : Endpoint<AddTrainerRequest>
{
    private readonly IAuleTechCommandHandler<AddTrainerCommand, Result> _commandHandler;

    public AddTrainerEndPoint(IAuleTechCommandHandler<AddTrainerCommand, Result> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(AddTrainerRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var result = await _commandHandler.HandleAsync(req.ToCommand(), ct);

        if (!result.Succeeded)
        {
            ThrowError(result.Errors.First());
        }
        
        await SendOkAsync(null, ct);
    }
}
