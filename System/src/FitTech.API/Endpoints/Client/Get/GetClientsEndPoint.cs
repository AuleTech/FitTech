using System.Security.Claims;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Dtos;
using FitTech.Application.Query.Client.Get;
using FitTech.Application.Query.Trainer.GetTrainerData;

namespace FitTech.API.Endpoints.Trainer;

[HttpGet("/Client/GetClients")]
public class GetClientsEndPoint : EndpointWithoutRequest<ClientDto> 
{
    private readonly IQueryHandler<GetClientQuery, Result<ClientDto>> _queryHandler;

    public GetClientsEndPoint(IQueryHandler<GetClientQuery, Result<ClientDto>> queryHandler)
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

        var Client = await _queryHandler.HandleAsync(new GetClientQuery(Guid.Parse(userId)), ct);

        if (!Client.Succeeded)
        {
            ThrowError(Client.Errors.First());
        }
        
        await SendAsync(Client.Value!, cancellation: ct);
    }
}
