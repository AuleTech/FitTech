using AuleTech.Core.Patterns;
using Blazored.LocalStorage;
using FitTech.API.Client;
using FitTech.ApiClient;
using FitTech.WebComponents.Authentication;
using FitTech.WebComponents.Models;
using Microsoft.Extensions.Logging;
using Result = AuleTech.Core.Patterns.Result;



namespace FitTech.WebComponents.Services;

internal sealed class UserService : IUserService
{
    private readonly FitTechAuthStateProvider _authStateProvider;
    private readonly IFitTechApiClient _fitTechApiClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly ILogger<UserService> _logger;
    

    public UserService(IFitTechApiClient fitTechApiClient, FitTechAuthStateProvider authStateProvider,
        ILogger<UserService> logger, ILocalStorageService localStorageService )
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
            var result = await _fitTechApiClient.LoginAsync(new LoginRequest() { Email = email, Password = password }, cancellationToken);

            if (!result.Succeeded)
            {
                return result.MapFailure<FitTechUser>();
            }
            
            var user = new FitTechUser { Email = email, AccessToken = result.Value?.AccessToken, RefreshToken = result.Value?.RefreshToken};

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

        var result = await _fitTechApiClient.RegisterAsync(
            new RegisterRequest() { Email = email, Password = password }, cancellationToken);

        return result.Succeeded
            ? Result.Success
            : Result.Failure(result.Errors.Select(x => x).ToArray());
    }

    public async Task<Result<string>> ForgotPasswordAsync(string to, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        var result = await _fitTechApiClient.ForgotPasswordAsync(
            new ForgotPasswordRequest()
            {
                Email = to, CallbackUrl = "NotNeededRightNow" //TODO: Add redirect url
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

        var result = await _fitTechApiClient.ResetPasswordAsync(
            new ResetPasswordRequest()
            {
                Email = email, Token = token, NewPassword = newPassword
            }, cancellationToken);

        return result;
    }

    public async Task<Result> LogoutAsync(CancellationToken cancellationToken)
    {
        await _localStorageService.RemoveItemAsync(FitTechUser.StorageKey, cancellationToken);
        return Result.Success;
    }
    
}
