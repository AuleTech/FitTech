using FastEndpoints;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;
using FitTech.Domain.Entities;

namespace FitTech.API.Auth.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
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
        var user = new FitTechUser { Email = req.Email, UserName = req.Email };

        var registrationResult =
            await _authenticationService.RegisterAsync(new RegisterUserDto(req.Email, req.Password), ct);

        var response = new RegisterResponse(registrationResult.Succeeded,
            registrationResult.Errors.Select(x => new RegisterErrors(x.Code, x.Description)));
        
        await SendAsync(response,
            registrationResult.Succeeded ? StatusCodes.Status200OK :
            registrationResult.Errors.Any() ? StatusCodes.Status500InternalServerError :
            StatusCodes.Status400BadRequest, ct);
    }
}
