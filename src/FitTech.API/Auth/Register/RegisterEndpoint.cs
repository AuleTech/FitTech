using FastEndpoints;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FitTech.API.Auth.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<FitTechUser> _userManager;

    public RegisterEndpoint(UserManager<FitTechUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Registers a new user";
        });
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = new FitTechUser { Email = req.Email, UserName = req.Email};

        var registrationResult = await _userManager.CreateAsync(user, req.Password).WaitAsync(ct);

        if (!registrationResult.Succeeded)
        {
            var response = new RegisterResponse(false,
                registrationResult.Errors.Select(x => new RegisterErrors(x.Code, x.Description)));
            await SendAsync(response, StatusCodes.Status500InternalServerError, ct);
            return;
        }

        await SendOkAsync(new RegisterResponse(true, []), ct);
    }
}
