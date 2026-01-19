using FitTech.ApiClient.Generated;
using Refit;

namespace FitTech.API.Client.ClientV2.Paths;

public interface IAuthenticationApiClient
{
    [Post("/auth/forgot-password")]
    Task<IApiResponse<string>> ForgotPasswordAsync([Body] ForgotPasswordRequest request,
        CancellationToken cancellationToken);

    [Post("/auth/login")]
    Task<IApiResponse<LoginResponse>> LoginAsync([Body] LoginRequest request, CancellationToken cancellationToken);

    [Post("/auth/refresh-token")]
    Task<IApiResponse<string>> RefreshTokenAsync([Body] RefreshTokenRequest request, CancellationToken cancellationToken);

    [Post("/auth/reset-password")]
    Task<IApiResponse> ResetPasswordAsync([Body] ResetPasswordRequest request, CancellationToken cancellationToken);
}
