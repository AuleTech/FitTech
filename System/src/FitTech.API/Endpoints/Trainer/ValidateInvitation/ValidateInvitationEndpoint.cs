using System.Security.Claims;
using FastEndpoints;
using FitTech.Application.Commands.Trainer.ValidateInvitation;

namespace FitTech.API.Endpoints.Trainer.ValidateInvitation;

[HttpGet("/trainer/validate-invitation")]
public class ValidateInvitationEndpoint : Endpoint<ValidateInvitationRequest, Guid>
{
    private readonly IValidateInvitationCommandHandler _commandHandler;

    public ValidateInvitationEndpoint(IValidateInvitationCommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(ValidateInvitationRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result =
            await _commandHandler.HandleAsync(new ValidateInvitationCommand(Guid.Parse(userId), req.Email, req.Code), ct);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                AddError(error);
            }
        }
        
        ThrowIfAnyErrors();
        await Send.OkAsync(result.Value, ct);
    }
}
