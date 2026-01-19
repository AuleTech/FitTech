using AuleTech.Core.Extensions.Refit;
using AuleTech.Core.Patterns.Result;
using FitTech.API.Client;
using FitTech.API.Client.ClientV2;
using FitTech.ApiClient;
using FitTech.ApiClient.Generated;
using FitTech.WebComponents.Authentication;
using FitTech.WebComponents.Models;
using FitTech.WebComponents.Persistence;
using Microsoft.Extensions.Logging;
using Result = AuleTech.Core.Patterns.Result.Result;


namespace FitTech.WebComponents.Services;

internal sealed class UserService : IUserService
{
    private readonly FitTechAuthStateProvider _authStateProvider;
    private readonly IFitTechApiClientV2 _fitTechApiClient;
    private readonly ILogger<UserService> _logger;
    private readonly IStorage _storage;


    public UserService(IFitTechApiClientV2 fitTechApiClient, FitTechAuthStateProvider authStateProvider,
        ILogger<UserService> logger, IStorage storage)
    {
        _fitTechApiClient = fitTechApiClient;
        _authStateProvider = authStateProvider;
        _logger = logger;
        _storage = storage;
    }

    public async Task<bool> IsLoggedAsync()
    {
        return await _storage.ContainsKeyAsync(FitTechUser.StorageKey, CancellationToken.None);
        //Let's keep it simple for now
    }

    public async Task<Result<FitTechUser>> LoginAsync(string email, string password,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        try
        {
            var response = await _fitTechApiClient.Auth.LoginAsync(new LoginRequest { Email = email, Password = password },
                cancellationToken);

            if (!response.IsSuccessful)
            {
                return response.ToFailedResult<LoginResponse,FitTechUser>();
            }

            var user = new FitTechUser
            {
                Email = email, AccessToken = response.Content?.AccessToken, RefreshToken = response.Content?.RefreshToken
            };

            await _storage.SetItemAsync(FitTechUser.StorageKey, user, cancellationToken);

            _authStateProvider.RaiseLoginEvent(user);

            return Result<FitTechUser>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during login: {Error}", ex.Message);
            return Result<FitTechUser>.Failure(["Los credenciales no son correctos"])!;
        }
    }

    public async Task<Result<string>> ForgotPasswordAsync(string to, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        var response = await _fitTechApiClient.Auth.ForgotPasswordAsync(
            new ForgotPasswordRequest
            {
                Email = to, CallbackUrl = "NotNeededRightNow" //TODO: Add redirect url
            }, cancellationToken);
        
        return response.ToResult();
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(newPassword);

        var response = await _fitTechApiClient.Auth.ResetPasswordAsync(
            new ResetPasswordRequest { Email = email, Token = token, NewPassword = newPassword }, cancellationToken);

        return response.ToResult();
    }

    public async Task<Result> LogoutAsync(CancellationToken cancellationToken)
    {
        await _storage.ClearAsync(cancellationToken);
        return Result.Success;
    }
}
