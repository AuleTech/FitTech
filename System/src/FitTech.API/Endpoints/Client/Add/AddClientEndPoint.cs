using System.Security.Claims;
using AuleTech.Core.Patterns;
using FastEndpoints;
using FitTech.Application.Dtos.Client;
using FitTech.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Client.Add;

[Authorize(AuthenticationSchemes = "Bearer")]
[HttpPost("/user/add-client")]
public class AddClientEndPoint : Endpoint<AddClientRequest, Result>
{
    private readonly IClientService _service;
    private readonly ILogger<AddClientEndPoint> _logger;

    public AddClientEndPoint(IClientService service, ILogger<AddClientEndPoint> logger)
    {
        _service = service;
        _logger = logger;
    }

    public override async Task HandleAsync(AddClientRequest req, CancellationToken ct)
    {
        //llega nulo, buscar manera de que recoja el AccesToken o el email del usuario
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        await _service.AddAsync(new AddClientDto(), ct);
        _logger.LogInformation("New client added");
        await SendOkAsync(Result.Success, ct);
        
    }
}
