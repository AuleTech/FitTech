using System.Security.Claims;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Services;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.User.AddClient;

[Authorize(AuthenticationSchemes = "Bearer")]
[HttpPost("/user/add-client")]
public class AddNewClientEndPoint : Endpoint<AddNewClientRequest, Result>
{
    private readonly NewClientService _service;
    private readonly ILogger<AddNewClientEndPoint> _logger;

    public AddNewClientEndPoint(NewClientService service, ILogger<AddNewClientEndPoint> logger)
    {
        _service = service;
        _logger = logger;
    }

    public override async Task HandleAsync(AddNewClientRequest req, CancellationToken ct)
    {
        //llega nulo, buscar manera de que recoja el AccesToken o el email del usuario
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var client = new Client(
            id: Guid.NewGuid(),
            nameUser: req.NameUser,
            lastNameuser: req.LastNameuser,
            eventDate: req.EventDate,
            emailUser: req.EmailUser,
            birthdate: req.Birthdate,
            phoneNumber: req.PhoneNumber,
            center: req.Center,
            trainingHours: req.TrainingHours,
            trainingModel: req.TrainingModel,
            subscriptionType: req.SubscriptionType,
            createdByUserId: userId!
        );
        
        await _service.NewClientAsync(client, ct);
        _logger.LogInformation("New client added");
        await SendOkAsync(Result.Success, ct);
        
    }
}
