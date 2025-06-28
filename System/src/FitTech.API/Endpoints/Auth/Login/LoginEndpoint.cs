using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;
using FitTech.Application.Commands.Auth.Login;
using Microsoft.AspNetCore.Authorization;

namespace FitTech.API.Endpoints.Auth.Login;

[AllowAnonymous]
[HttpPost("/auth/login")]
public sealed class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuleTechCommandHandler<LoginCommand, Result<LoginResultDto>> _commandHandler;

    public LoginEndpoint(IAuleTechCommandHandler<LoginCommand, Result<LoginResultDto>> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _commandHandler.HandleAsync(new LoginCommand(req.Email, req.Password), ct);

        if (!result.Succeeded)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendAsync(new LoginResponse(result.Value!.AccessToken!, result.Value.RefreshToken), cancellation: ct);
    }
}
