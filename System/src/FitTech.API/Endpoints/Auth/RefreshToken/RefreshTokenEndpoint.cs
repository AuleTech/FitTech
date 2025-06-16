using AuleTech.Core.Patterns;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.RefreshToken;

[HttpPost("/auth/refresh-token")]
[AllowAnonymous]
public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, Result<string>>
{
    private readonly IFitTechAuthenticationService _authenticationService;

    public RefreshTokenEndpoint(IFitTechAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var result =
            await _authenticationService.RefreshTokenAsync(
                new RefreshTokenDto(req.RefreshToken, req.ExpiredAccessToken), ct);

        if (!result.Succeeded)
        {
            await SendAsync(result.MapFailure<string>(),
                StatusCodes.Status400BadRequest, ct);

            return;
        }

        if (result.Value!.NeedLoginAgain)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendAsync(result.Map(x => x?.AccessToken), cancellation: ct);
    }
}
