using Blazored.LocalStorage;
using FitTech.Api.Client.Generated;
using FitTech.WebComponents.Authentication;
using FitTech.WebComponents.Models;
using Microsoft.Extensions.Logging;
using Result = FitTech.WebComponents.Models.Result;
using FitTech.Application.Auth.Services;


namespace FitTech.WebComponents.Services;

internal sealed class UserService : IUserService
{
    private readonly FitTechAuthStateProvider _authStateProvider;
    private readonly FitTechAPIClient _fitTechApiClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly ILogger<UserService> _logger;
    private readonly IEmailService _emailService;

    public UserService(FitTechAPIClient fitTechApiClient, FitTechAuthStateProvider authStateProvider,
        ILogger<UserService> logger, ILocalStorageService localStorageService, IEmailService emailService )
    {
        _fitTechApiClient = fitTechApiClient;
        _authStateProvider = authStateProvider;
        _logger = logger;
        _localStorageService = localStorageService;
        _emailService = emailService;
    }

    public async Task<bool> IsLoggedAsync() => await _localStorageService.ContainKeyAsync(FitTechUser.StorageKey); //Let's keep it simple for now

    public async Task<Result<FitTechUser>> LoginAsync(string email, string password,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        try
        {
            var result = await _fitTechApiClient.LoginEndpointAsync(
                new LoginRequest() { Email = email, Password = password }, cancellationToken);

            var user = new FitTechUser { Email = email, AccessToken = result.Value.AccessToken, RefreshToken =result.Value.RefreshToken};

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

        var result = await _fitTechApiClient.RegisterEndpointAsync(
            new RegisterRequest() { Email = email, Password = password }, cancellationToken);

        return result.Succeeded
            ? Result.Success
            : Result.Failure(result.Errors.Select(x => x).ToArray());
    }

    public async Task<Result<string>> ForgotPasswordAsync(string to, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        var result = await _fitTechApiClient.ForgotPasswordEndpointAsync(
            new ForgotPasswordRequest()
            {
                Email = to, CallbackUrl = "NotNeededRightNow" //TODO: Add redirect url
            }, cancellationToken);
        
        if (result.Succeeded)
        {
            var htmlbody = $"https://yourfrontend.com/reset-password?token={result.Value}";
            var subject = "Reset Password FitTech";

            await _emailService.SendEmailAsync(to, subject, htmlbody);
        }
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

        var result = await _fitTechApiClient.ResetPasswordEndpointAsync(
            new ResetPasswordRequest()
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
