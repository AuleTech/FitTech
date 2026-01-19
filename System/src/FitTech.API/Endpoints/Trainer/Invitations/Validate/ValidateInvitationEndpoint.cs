using FastEndpoints;
using FitTech.Application.Commands.Trainer.ValidateInvitation;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Trainer.Invitations.Validate;

[AllowAnonymous]
[HttpGet("/trainer/invitations/validate")]
public class ValidateInvitationEndpoint : Endpoint<ValidateInvitationRequest, Guid>
{
    private readonly IValidateInvitationCommandHandler _commandHandler;

    public ValidateInvitationEndpoint(IValidateInvitationCommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(ValidateInvitationRequest req, CancellationToken ct)
    {
        var result =
            await _commandHandler.HandleAsync(new ValidateInvitationCommand(req.Email, req.Code), ct);

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
