using Blazored.LocalStorage;
using FitTech.API.Client.Contracts;
using FitTech.WebComponents.Authentication;
using FitTech.WebComponents.Models;
using Microsoft.Extensions.Logging;

namespace FitTech.WebComponents.Services;

internal sealed class UserService : IUserService
{
    private readonly FitTechAuthStateProvider _authStateProvider;
    private readonly IFitTechApiClient _fitTechApiClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly ILogger<UserService> _logger;

    public UserService(IFitTechApiClient fitTechApiClient, FitTechAuthStateProvider authStateProvider,
        ILogger<UserService> logger, ILocalStorageService localStorageService)
    {
        _fitTechApiClient = fitTechApiClient;
        _authStateProvider = authStateProvider;
        _logger = logger;
        _localStorageService = localStorageService;
    }

    public async Task<Result<FitTechUser>> LoginAsync(string email, string password,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        try
        {
            var result = await _fitTechApiClient.FitTechAPIAuthLoginLoginEndpointAsync(
                new FitTechAPIAuthLoginLoginRequest { Email = email, Password = password }, cancellationToken);

            var user = new FitTechUser { Email = email, AccessToken = result.AccessToken };

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
            : Result.Failure(result.Errors.Select(x => x.Description).ToArray());
    }

    public Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // ArgumentException.ThrowIfNullOrWhiteSpace(email);
        // var result = await _fitTechApiClient.FitTechAPIAuthRecoveryRecoveryEndpointAsync(
        //     new FitTechAPIAuthRecoveryRecoveryRequest { Email = email }, cancellationToken);
        //
        // return result.Succeeded
        //     ? Result.Success
        //     : Result.Failure(result.Errors.Select(x => x.Description).ToArray());
    }

    public Task<Result> ResetAsync(string password, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // ArgumentException.ThrowIfNullOrWhiteSpace(email);
        // var result = await _fitTechApiClient.FitTechAPIAuthRecoveryRecoveryEndpointAsync(
        //     new FitTechAPIAuthRecoveryRecoveryRequest { Email = email }, cancellationToken);
        //
        // return result.Succeeded
        //     ? Result.Success
        //     : Result.Failure(result.Errors.Select(x => x.Description).ToArray());
    }

}
