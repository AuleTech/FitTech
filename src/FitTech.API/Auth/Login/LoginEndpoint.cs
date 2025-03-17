using FastEndpoints;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;

namespace FitTech.API.Auth.Login;

public sealed class LoginEndpoint : Endpoint<LoginRequest, Result<LoginResponse>>
{
    private readonly IFitTechAuthenticationService _authenticationService;

    public LoginEndpoint(IFitTechAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _authenticationService.LoginAsync(new LoginDto(req.Email, req.Password), ct);

        if (!result.Succeeded)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendAsync(new LoginResponse(result.Value!.AccessToken!), cancellation: ct);
    }
}
