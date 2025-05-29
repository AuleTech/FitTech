using FastEndpoints;
using FitTech.Application.Services;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.User.AddClient;

[AllowAnonymous]
public sealed class AddNewClientEndPoint : Endpoint<Client>
{
    private readonly AddClientService _service;
    private readonly ILogger<AddNewClientEndPoint> _logger;

    public AddNewClientEndPoint(AddClientService service, ILogger<AddNewClientEndPoint> logger)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/User/AddClient");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Adds a new client to the system.";
            s.Description = "This endpoint adds a new client to the database using the provided client data.";
        });
    }

    public override async Task HandleAsync(Client client, CancellationToken ct)
    {
        await _service.AddNewClientAsync(client, ct);
        _logger.LogInformation("New client added");
        await SendOkAsync(ct);
    }
}
