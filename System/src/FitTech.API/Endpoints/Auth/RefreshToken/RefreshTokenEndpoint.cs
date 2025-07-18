﻿using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Query.Auth.RefreshToken;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.RefreshToken;

[HttpPost("/auth/refresh-token")]
[AllowAnonymous]
public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, string>
{
    private readonly IQueryHandler<RefreshTokenQuery, Result<RefreshTokenResultDto>> _queryHandler;

    public RefreshTokenEndpoint(IQueryHandler<RefreshTokenQuery, Result<RefreshTokenResultDto>> queryHandler)
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
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendAsync(result.Value!.AccessToken!, cancellation: ct);
    }
}
