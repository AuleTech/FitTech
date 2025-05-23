﻿using System.Security.Claims;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Dtos;
using FitTech.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.User.GetCurrent;

[Authorize(AuthenticationSchemes = "Bearer")]
[HttpGet("/user/get-current")]
public class GetCurrentUserEndpoint : EndpointWithoutRequest<Result<UserInfoDto>>
{
    private readonly IUserService _userService;

    public GetCurrentUserEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var userInfo = await _userService.GetUserInfoAsync(Guid.Parse(userId), ct);

        await SendAsync(userInfo, cancellation: ct);
    }
}
