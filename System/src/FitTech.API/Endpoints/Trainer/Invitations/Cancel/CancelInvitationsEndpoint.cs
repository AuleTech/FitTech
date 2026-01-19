using System.Security.Claims;
using FastEndpoints;
using FitTech.API.Endpoints.Trainer.Invitations.InviteClient;
using FitTech.Application.Commands.Trainer.CancelInvitations;

namespace FitTech.API.Endpoints.Trainer.Invitations.Cancel;

[HttpPost("/trainer/invitations/cancel")]
public class CancelInvitationsEndpoint : Endpoint<InviteClientRequest>
{
    private readonly ICancelInvitationsCommandHandler _commandHandler;

    public CancelInvitationsEndpoint(ICancelInvitationsCommandHandler commandHandler)
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

        var result = await _commandHandler.HandleAsync(new CancelInvitationsCommand(Guid.Parse(userId), req.ClientEmail), ct);

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

