using AuleTech.Core.Patterns.Result;
using FitTech.ApiClient;
using Result = AuleTech.Core.Patterns.Result.Result;

namespace FitTech.API.Client;

public interface IFitTechApiClient
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<Result<string>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest,
        CancellationToken cancellationToken);

    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest,
        CancellationToken cancellationToken);

    Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken);
    Task<Result<UserInfoDto>> GetCurrentUserAsync(CancellationToken cancellationToken);
    Task<Result<TrainerDataDto>> GetTrainerDataAsync(CancellationToken cancellationToken);
    Task<Result> RegisterTrainerAsync(RegisterTrainerRequest request, CancellationToken cancellationToken);
    Task<Result> SendInvitationAsync(InviteClientRequest request, CancellationToken cancellationToken);
    Task<Result<GetInvitationsResponse>> GetInvitationsAsync(CancellationToken cancellationToken);
}
