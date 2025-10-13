using System.Security.Claims;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Commands.Trainer.Register;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Trainer.Register;

[AllowAnonymous]
[HttpPost("/trainer/register")]
public class RegisterTrainerEndpoint : Endpoint<RegisterTrainerRequest>
{
    private readonly IAuleTechCommandHandler<RegisterTrainerCommand, Result> _commandHandler;

    public RegisterTrainerEndpoint(IAuleTechCommandHandler<RegisterTrainerCommand, Result> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(RegisterTrainerRequest req, CancellationToken ct)
    {
        var result = await _commandHandler.HandleAsync(req.ToCommand(), ct);

        if (!result.Succeeded)
        {
            ThrowError(result.Errors.First());
        }

        await Send.OkAsync(null, ct);
    }
}
