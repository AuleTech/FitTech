using System.Security.Claims;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Query.Client.GetSettings;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Client; 

[Authorize(AuthenticationSchemes = "Bearer")]
[HttpGet("/client/settings")]
public class GetSettingsEndpoint : EndpointWithoutRequest<ClientSettingsDto> //TODO: We shouldn't be returning Application Dtos
{
    private readonly IQueryHandler<GetClientSettingsQuery, Result<ClientSettingsDto>> _queryHandler;

    public GetSettingsEndpoint(IQueryHandler<GetClientSettingsQuery, Result<ClientSettingsDto>> queryHandler)
    {
        _queryHandler = queryHandler;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var clientSettings = await _queryHandler.HandleAsync(new GetClientSettingsQuery(Guid.Parse(userId)), ct);

        if (!clientSettings.Succeeded)
        {
            ThrowError(clientSettings.Errors.First());
        }
        
        await SendAsync(clientSettings.Value!, cancellation: ct);
    }
}
