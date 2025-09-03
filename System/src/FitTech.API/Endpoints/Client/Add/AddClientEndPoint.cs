using System.Security.Claims;
using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Commands.Client.Add;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Client.Add;

[HttpPost("/user/add-client")]
public class AddClientEndPoint : Endpoint<AddClientRequest>
{
    private readonly IAuleTechCommandHandler<AddClientCommand, Result> _commandHandler;

    public AddClientEndPoint(IAuleTechCommandHandler<AddClientCommand, Result> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(AddClientRequest req, CancellationToken ct)
    {
        //TODO: Probably add Id when created or something 
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var command = req.ToCommand(Guid.Parse(userId));
        var result = await _commandHandler.HandleAsync(command, ct);
        

        if (!result.Succeeded)
        {
            ThrowError(result.Errors.First());
        }
        
        await SendOkAsync(null, ct);
    }
}
