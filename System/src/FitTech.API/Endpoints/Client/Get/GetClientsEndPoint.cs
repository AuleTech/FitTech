using System.Security.Claims;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Dtos;
using FitTech.Application.Query.Client.Get;

namespace FitTech.API.Endpoints.Client.Get;

[HttpGet("/Client/GetClients")]
public class GetClientsEndPoint : EndpointWithoutRequest<List<ClientDataDto>>
{
    private readonly IListQueryHandler<GetClientDataQuery, ClientDataDto> _queryHandler;

    public GetClientsEndPoint(IListQueryHandler<GetClientDataQuery, ClientDataDto> queryHandler)
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

        var Client = await _queryHandler.HandleGroupAsync(new GetClientDataQuery(Guid.Parse(userId)), ct);

        if (!Client.Succeeded)
        {
            ThrowError(Client.Errors.First());
        }
        
        await SendAsync(Client.Value!, cancellation: ct);
    }
}
