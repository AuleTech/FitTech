using AuleTech.Core.Messaging;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Commands.Auth.Login;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.Login;

[AllowAnonymous]
[HttpPost("/auth/login")]
public sealed class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly ILoginCommandHandler _commandHandler;

    public LoginEndpoint(ILoginCommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _commandHandler.HandleAsync(new LoginCommand(req.Email, req.Password), ct);

        if (!result.Succeeded)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        await Send.OkAsync(new LoginResponse(result.Value!.AccessToken!, result.Value.RefreshToken), ct);
    }
}
