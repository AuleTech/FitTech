using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Commands.Auth.ResetPassword;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.ResetPassword;

[HttpPost("/auth/reset-password")]
[AllowAnonymous]
public class ResetPasswordEndpoint : Endpoint<ResetPasswordRequest>
{
    private readonly IAuleTechCommandHandler<ResetPasswordCommand, Result> _commandHandler;

    public ResetPasswordEndpoint(IAuleTechCommandHandler<ResetPasswordCommand, Result> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
    {
        var result =
            await _commandHandler.HandleAsync(new ResetPasswordCommand(req.Email, req.NewPassword, req.Token),
                ct);

        if (!result.Succeeded)
        {
            ThrowError(result.Errors.First());
        }

        await Send.NoContentAsync(ct);
    }
}
