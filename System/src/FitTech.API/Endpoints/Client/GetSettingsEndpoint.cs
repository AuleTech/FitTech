using System.Security.Claims;
using AuleTech.Core.Patterns;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Dtos.Client;
using FitTech.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Client;

[HttpGet("/client/settings")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class GetSettingsEndpoint : EndpointWithoutRequest<Result<ClientSettingsDto>>
{
    private readonly IClientService _clientService;

    public GetSettingsEndpoint(IClientService clientService)
    {
        _clientService = clientService;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var clientSettings = await _clientService.GetSettingsAsync(Guid.Parse(userId), ct);

        await SendAsync(clientSettings, cancellation: ct);
    }
}
