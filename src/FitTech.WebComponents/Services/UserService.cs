using Blazored.LocalStorage;
using FitTech.API.Client;
using FitTech.API.Contracts;
using FitTech.WebComponents.Authentication;
using FitTech.WebComponents.Models;
using Microsoft.Extensions.Logging;

namespace FitTech.WebComponents.Services;

internal sealed class UserService : IUserService
{
    private readonly FitTechAuthStateProvider _authStateProvider;
    private readonly FitTechAPIClient _fitTechApiClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly ILogger<UserService> _logger;

    public UserService(FitTechAPIClient fitTechApiClient, FitTechAuthStateProvider authStateProvider,
        ILogger<UserService> logger, ILocalStorageService localStorageService)
    {
        _fitTechApiClient = fitTechApiClient;
        _authStateProvider = authStateProvider;
        _logger = logger;
        _localStorageService = localStorageService;
    }

    public async Task<bool> IsLoggedAsync() => await _localStorageService.ContainKeyAsync(FitTechUser.StorageKey); //Let's keep it simple for now

    public async Task<Result<FitTechUser>> LoginAsync(string email, string password,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        try
        {
            var result = await _fitTechApiClient.FitTechAPIAuthLoginLoginEndpointAsync(
                new FitTechAPIAuthLoginLoginRequest { Email = email, Password = password }, cancellationToken);

            var user = new FitTechUser { Email = email, AccessToken = result.Value.AccessToken };

            await _localStorageService.SetItemAsync(FitTechUser.StorageKey, user, cancellationToken);
            
            _authStateProvider.RaiseLoginEvent(user);

            return Result<FitTechUser>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during login: {Error}", ex.Message);
            return Result<FitTechUser>.Failure(["Los credenciales no son correctos"])!;
        }
    }

    public async Task<Result> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var result = await _fitTechApiClient.FitTechAPIAuthRegisterRegisterEndpointAsync(
            new FitTechAPIAuthRegisterRegisterRequest { Email = email, Password = password }, cancellationToken);

        return result.Succeeded
            ? Result.Success
            : Result.Failure(result.Errors.Select(x => x).ToArray());
    }

    public async Task<Result<string>> ForgotPasswordAsync(string email, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        var result = await _fitTechApiClient.FitTechAPIAuthForgotPasswordForgotPasswordEndpointAsync(
            new FitTechAPIAuthForgotPasswordForgotPasswordRequest
            {
                Email = email, CallbackUrl = "NotNeededRightNow" //TODO: Add redirect url
            }, cancellationToken);

        return new Result<string>()
        {
            Errors = result.Errors.ToArray(),
            Succeeded = result.Succeeded,
            Value = result.Value
        };
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(newPassword);

        var result = await _fitTechApiClient.FitTechAPIAuthResetPasswordResetPasswordEndpointAsync(
            new FitTechAPIAuthResetPasswordResetPasswordRequest
            {
                Email = email, Token = token, NewPassword = newPassword
            }, cancellationToken);

        return new Result() { Errors = result.Errors.ToArray(), Succeeded = result.Succeeded };
    }

    public async Task<Result> LogoutAsync(CancellationToken cancellationToken)
    {
        await _localStorageService.RemoveItemAsync(FitTechUser.StorageKey, cancellationToken);
        return Result.Success;
    }
}
