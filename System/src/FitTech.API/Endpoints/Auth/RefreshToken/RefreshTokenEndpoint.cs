using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Query.Auth.RefreshToken;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.RefreshToken;

[HttpPost("/auth/refresh-token")]
[AllowAnonymous]
public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, string>
{
    private readonly IRefreshTokenQueryHandler _queryHandler;

    public RefreshTokenEndpoint(IRefreshTokenQueryHandler queryHandler)
    {
        _queryHandler = queryHandler;
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var result =
            await _queryHandler.HandleAsync(
                new RefreshTokenQuery(req.RefreshToken, req.ExpiredAccessToken), ct);

        if (!result.Succeeded)
        {
            ThrowError(result.Errors.First());
        }

        if (result.Value!.NeedLoginAgain)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        await Send.OkAsync(result.Value!.AccessToken!, ct);
    }
}
