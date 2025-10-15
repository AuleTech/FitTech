using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FastEndpoints;
using FitTech.Application.Commands.Trainer.InviteClient;

namespace FitTech.API.Endpoints.Trainer.InviteClient;

[HttpPost("/trainer/invite-client")]
public class InviteClientEndpoint : Endpoint<InviteClientRequest>
{
    private readonly IInviteClientCommandHandler _commandHandler;

    public InviteClientEndpoint(IInviteClientCommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(InviteClientRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result = await _commandHandler.HandleAsync(new InviteClientCommand(Guid.Parse(userId), req.ClientEmail), ct);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                AddError(error);
            }
        }
        
        ThrowIfAnyErrors();
        
        await Send.NoContentAsync(ct);
    }
}
