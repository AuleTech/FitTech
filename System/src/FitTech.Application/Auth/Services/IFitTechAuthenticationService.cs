using AuleTech.Core.Patterns;
using FitTech.Application.Auth.Dtos;

namespace FitTech.Application.Auth.Services;

public interface IFitTechAuthenticationService
{
    Task<Result> RegisterAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken);
    Task<Result<LoginResultDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto, CancellationToken cancellationToken);
    Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken);
    Task<Result<RefreshTokenResultDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken);
}
