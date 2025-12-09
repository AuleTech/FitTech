using AuleTech.Core.Patterns.Result;
using FitTech.ApiClient;
using FitTech.ApiClient.Generated;
using Result = AuleTech.Core.Patterns.Result.Result;

namespace FitTech.API.Client;

//TODO: Refactor
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
    Task<Result> RegisterClientAsync(RegisterClientRequest request, CancellationToken cancellationToken);
    Task<Result<Guid>> ValidateInvitationAsync(string email, int code, CancellationToken cancellationToken);
    Task<Result> CancelInvitationsAsync(InviteClientRequest request, CancellationToken cancellationToken);
}
