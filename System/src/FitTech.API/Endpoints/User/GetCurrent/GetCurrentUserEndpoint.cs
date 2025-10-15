using System.Security.Claims;
using FastEndpoints;
using FitTech.Application.Dtos;
using FitTech.Application.Services;

namespace FitTech.API.Endpoints.User.GetCurrent;

[HttpGet("/user/get-current")]
public class GetCurrentUserEndpoint : EndpointWithoutRequest<UserInfoDto>
{
    private readonly IUserService _userService;
    private readonly ILogger<GetCurrentUserEndpoint> _logger;
    public GetCurrentUserEndpoint(IUserService userService, ILogger<GetCurrentUserEndpoint> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var userInfo = await _userService.GetUserInfoAsync(Guid.Parse(userId), ct);

        if (!userInfo.Succeeded)
        {
            ThrowError(userInfo.Errors.First());
        }

        await Send.OkAsync(userInfo.Value!, ct);
    }
}
