using System.Security.Claims;
using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Dtos;
using FitTech.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.User.GetCurrent;

[HttpGet("/user/get-current")]
public class GetCurrentUserEndpoint : EndpointWithoutRequest<UserInfoDto>
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

        if (!userInfo.Succeeded)
        {
            ThrowError(userInfo.Errors.First());
        }
        
        await SendAsync(userInfo.Value!, cancellation: ct);
    }
}
