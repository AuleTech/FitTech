using System.Security.Claims;
using FastEndpoints;

namespace FitTech.API.Endpoints.Trainer.InviteClient;

[HttpPost("/trainer/invite-client")]
public class InviteClientEndpoint : Endpoint<InviteClientRequest>
{
    public override async Task HandleAsync(InviteClientRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }
    }
}
