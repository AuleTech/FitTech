using AuleTech.Core.Patterns;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.ResetPassword;

[HttpPost("/auth/reset-password")]
[AllowAnonymous]
public class ResetPasswordEndpoint : Endpoint<ResetPasswordRequest, Result>
{
    private readonly IFitTechAuthenticationService _authenticationService;

    public ResetPasswordEndpoint(IFitTechAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
    {
        var result =
            await _authenticationService.ResetPasswordAsync(new ResetPasswordDto(req.Email, req.NewPassword, req.Token),
                ct);

        await SendAsync(result, result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, ct);
    }
}
