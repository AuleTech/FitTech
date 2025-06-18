using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Commands.Auth.ForgotPassword;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.ForgotPassword;

[AllowAnonymous]
[HttpPost("/auth/forgot-password")]
public class ForgotPasswordEndpoint : Endpoint<ForgotPasswordRequest, string>
{
    private readonly IAuleTechCommandHandler<ForgotPasswordCommand, Result<string>>
        _auleTechCommandHandler;

    public ForgotPasswordEndpoint(IAuleTechCommandHandler<ForgotPasswordCommand, Result<string>> auleTechCommandHandler)
    {
        _auleTechCommandHandler = auleTechCommandHandler;
    }

    //TODO: Add validation
    public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
    {
        var result =
            await _auleTechCommandHandler.HandleAsync(new ForgotPasswordCommand(req.Email, req.CallbackUrl), ct);

        if (!result.Succeeded)
        {
            ThrowError(result.Errors.First()); //TODO: Just for now
        }

        await SendAsync(result.Value!, cancellation: ct);
    }
}
