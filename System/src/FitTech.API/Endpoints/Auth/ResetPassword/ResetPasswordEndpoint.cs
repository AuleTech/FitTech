using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;
using FitTech.Application.Commands.Auth.RequestPassword;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.ResetPassword;

[HttpPost("/auth/reset-password")]
[AllowAnonymous]
public class ResetPasswordEndpoint : Endpoint<ResetPasswordRequest, Result>
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

        await SendAsync(result, result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, ct);
    }
}
