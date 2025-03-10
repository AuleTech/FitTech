using FitTech.Application.Auth.Dtos;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Auth.Services;

public interface IFitTechAuthenticationService
{
    Task<IdentityResult> RegisterAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken);
    Task<LoginResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
}
