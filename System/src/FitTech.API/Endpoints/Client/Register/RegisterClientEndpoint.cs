using System.Security.Claims;
using FastEndpoints;
using FitTech.Application.Commands.Client.Register;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Client.Register;

[HttpPost("/client/register")]
[AllowAnonymous]
public class RegisterClientEndpoint : Endpoint<RegisterClientRequest>
{
    private readonly IRegisterClientCommandHandler _clientCommandHandler;

    public RegisterClientEndpoint(IRegisterClientCommandHandler clientCommandHandler)
    {
        _clientCommandHandler = clientCommandHandler;
    }

    public override async Task HandleAsync(RegisterClientRequest req, CancellationToken ct)
    {
        var result = await _clientCommandHandler.HandleAsync(
            new RegisterClientCommand(req.InvitationId, req.Information, req.Credentials, req.Address,
                req.TrainingSettings, req.BodyMeasurement), ct);

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
