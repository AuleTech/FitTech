using System.Security.Claims;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Commands.Trainer.Register;

namespace FitTech.API.Endpoints.Trainer.Register;

[HttpPost("/user/add-trainer")]
public class RegisterTrainerEndPoint : Endpoint<RegisterTrainerRequest>
{
    private readonly IAuleTechCommandHandler<RegisterTrainerCommand, Result> _commandHandler;

    public RegisterTrainerEndPoint(IAuleTechCommandHandler<RegisterTrainerCommand, Result> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(RegisterTrainerRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result = await _commandHandler.HandleAsync(req.ToCommand(), ct);

        if (!result.Succeeded)
        {
            ThrowError(result.Errors.First());
        }

        await Send.OkAsync(null, ct);
    }
}
