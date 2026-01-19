using System.Security.Claims;
using FastEndpoints;
using FitTech.API.Endpoints.Trainer.Invitations.InviteClient;
using FitTech.Application.Commands.Trainer.ResendInvitations;

namespace FitTech.API.Endpoints.Trainer.Invitations.Resend;

[HttpPost("/trainer/invitations/resend")]
public class ResendInvitationsEndpoint : Endpoint<InviteClientRequest>
{
    private readonly IResendInvitationsCommandHandler _commandHandler;

    public ResendInvitationsEndpoint(IResendInvitationsCommandHandler commandHandler)
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

        var result = await _commandHandler.HandleAsync(new ResendInvitationsCommand(Guid.Parse(userId), req.ClientEmail), ct);

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
