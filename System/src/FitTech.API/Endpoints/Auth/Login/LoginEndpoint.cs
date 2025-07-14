using AuleTech.Core.Messaging;
using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Commands.Auth.Login;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.Login;

[AllowAnonymous]
[HttpPost("/auth/login")]
public sealed class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuleTechCommandHandler<LoginCommand, Result<LoginResultDto>> _commandHandler;
    private readonly IAuleTechQueuePublisher _publisher;

    public LoginEndpoint(IAuleTechCommandHandler<LoginCommand, Result<LoginResultDto>> commandHandler, IAuleTechQueuePublisher publisher)
    {
        _commandHandler = commandHandler;
        _publisher = publisher;
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        await _publisher.PublishAsync("Comemela Gerardo", ct);
        var result = await _commandHandler.HandleAsync(new LoginCommand(req.Email, req.Password), ct);

        if (!result.Succeeded)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendAsync(new LoginResponse(result.Value!.AccessToken!, result.Value.RefreshToken), cancellation: ct);
    }
}
