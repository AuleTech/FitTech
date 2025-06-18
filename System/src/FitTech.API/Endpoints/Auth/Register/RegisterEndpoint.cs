using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;
using FitTech.Application.Commands.Auth.Register;

namespace FitTech.API.Endpoints.Auth.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest>
{

    private readonly IAuleTechCommandHandler<RegisterCommand, Result> _commandHandler;

    public RegisterEndpoint(IAuleTechCommandHandler<RegisterCommand, Result> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var registrationResult =
            await _commandHandler.HandleAsync(new RegisterCommand(req.Email, req.Password), ct);


        if (!registrationResult.Succeeded)
        {
            ThrowError(registrationResult.Errors.First());
        }

        await SendAsync(null,cancellation: ct);
    }
}
