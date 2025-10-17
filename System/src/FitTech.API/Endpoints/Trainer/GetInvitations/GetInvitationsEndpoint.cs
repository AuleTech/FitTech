using System.Security.Claims;
using FastEndpoints;
using FitTech.Application.Query.Trainer.GetInvitations;

namespace FitTech.API.Endpoints.Trainer.GetInvitations;

[HttpGet("/trainer/invitations")]
public class GetInvitationsEndpoint : EndpointWithoutRequest<GetInvitationsResponse>
{
    private readonly IGetInvitationQueryHandler _queryHandler;

    public GetInvitationsEndpoint(IGetInvitationQueryHandler queryHandler)
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

        var result = await _queryHandler.HandleAsync(new GetInvitationsQuery(Guid.Parse(userId)), ct);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                AddError(error);
            }
        }
        
        ThrowIfAnyErrors();

        await Send.OkAsync(new GetInvitationsResponse(result.Value!), ct);
    }
}
