using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;

namespace FitTech.API.Auth.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest, Result>
{
    private readonly IFitTechAuthenticationService _authenticationService;

    public RegisterEndpoint(IFitTechAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var registrationResult =
            await _authenticationService.RegisterAsync(new RegisterUserDto(req.Email, req.Password), ct);
        
        
        await SendAsync(registrationResult,
            registrationResult.Succeeded ? StatusCodes.Status200OK :
            registrationResult.Errors.Any() ? StatusCodes.Status400BadRequest :
            StatusCodes.Status500InternalServerError, ct);
    }
}
