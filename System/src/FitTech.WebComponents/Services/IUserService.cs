using AuleTech.Core.Patterns.Result;
using FitTech.ApiClient;
using FitTech.ApiClient.Generated;
using FitTech.WebComponents.Models;

namespace FitTech.WebComponents.Services;

public interface IUserService
{
    Task<bool> IsLoggedAsync();
    Task<Result<FitTechUser>> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<Result<string>> ForgotPasswordAsync(string email, CancellationToken cancellationToken);

    Task<Result> ResetPasswordAsync(string email, string token, string newPassword,
        CancellationToken cancellationToken);

    Task<Result> LogoutAsync(CancellationToken cancellationToken);
    Task<Result<TrainerDataDto>> GetTrainerDataAsync(CancellationToken cancellationToken);
    Task<Result<GetInvitationsResponse>> GetInvitationsTrainer(CancellationToken cancellationToken);
}
