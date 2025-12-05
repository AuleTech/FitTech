using System.Security.Claims;
using FastEndpoints;
using FitTech.API.Endpoints.Trainer.CancelInvitations;
using FitTech.Application.Commands.Trainer.CancelInvitations;


namespace FitTech.API.Endpoints.Trainer.CancelInvitations;

[HttpGet("/trainer/cancelinvitations")]
public class CancelInvitationsEndpoint : EndpointWithoutRequest<CancelInvitationsResponse>
{
    private readonly ICancelInvitationCommandHandler _queryHandler;

    public CancelInvitationsEndpoint(ICancelInvitationCommandHandler queryHandler)
    {
        _queryHandler = queryHandler;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result = await _queryHandler.HandleAsync(new CancelInvitationsCommand(Guid.Parse(userId)), ct);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                AddError(error);
            }
        }
        
        ThrowIfAnyErrors();

        await Send.OkAsync(new CancelInvitationsResponse(result.Value!), ct);
    }
}

