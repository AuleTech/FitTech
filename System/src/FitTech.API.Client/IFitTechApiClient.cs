using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using FitTech.ApiClient;
using Result = AuleTech.Core.Patterns.Result.Result;

namespace FitTech.API.Client;

public interface IFitTechApiClient
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken);
    Task<Result<string>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest, CancellationToken cancellationToken);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken);
    Task<Result<UserInfoDto>> GetCurrentUserAsync(CancellationToken cancellationToken);
    Task<Result> AddNewClientAsync(AddClientRequest addNewClientRequest, CancellationToken cancellationToken);
    Task<Result<ClientSettingsDto>> GetClientSettings(CancellationToken cancellationToken);
    Task<Result<TrainerDataDto>> GetTrainerDataAsync(CancellationToken cancellationToken);
    Task<Result> UpdateUserConfigurationAsync(UpdateUSerConfigurationRequest updateUserConfigurationRequest, CancellationToken cancellationToken);
    Task<Result<ICollection<ClientDataDto>>> GetClients(CancellationToken cancellationToken);
}
