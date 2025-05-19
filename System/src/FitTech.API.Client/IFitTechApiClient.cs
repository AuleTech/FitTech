using AuleTech.Core.Patterns;
using FitTech.ApiClient;
using Result = AuleTech.Core.Patterns.Result;

namespace FitTech.API.Client;

public interface IFitTechApiClient
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken);
    Task<Result<string>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest, CancellationToken cancellationToken);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken);
    Task<Result> GetCurrentUserAsync(CancellationToken cancellationToken);
}
