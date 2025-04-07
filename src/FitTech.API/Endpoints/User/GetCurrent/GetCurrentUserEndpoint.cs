using FastEndpoints;
using FitTech.Application;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.User.GetCurrent;

[Authorize]
[HttpGet("/user/get-current")]
public class GetCurrentUserEndpoint : EndpointWithoutRequest<Result>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync(Result.Success, cancellation: ct);
    }
}
