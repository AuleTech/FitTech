using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.ForgotPassword;

[AllowAnonymous]
[HttpPost("/auth/forgot-password")]
public class ForgotPasswordEndpoint: Endpoint<ForgotPasswordRequest, Result<string>>
{
    private readonly IFitTechAuthenticationService _authenticationService;

    public ForgotPasswordEndpoint(IFitTechAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
    {
        var result =
            await _authenticationService.ForgotPasswordAsync(new ForgotPasswordDto(req.Email, req.CallbackUrl), ct);

        await SendAsync(result, result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, ct);
    }
}
