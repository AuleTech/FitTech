using AuleTech.Core.Patterns.Result;
using FitTech.API.Client;
using FitTech.ApiClient;
using FitTech.WebComponents.Authentication;
using FitTech.WebComponents.Models;
using FitTech.WebComponents.Persistence;
using Microsoft.Extensions.Logging;
using Result = AuleTech.Core.Patterns.Result.Result;


namespace FitTech.WebComponents.Services;

internal sealed class UserService : IUserService
{
    private readonly FitTechAuthStateProvider _authStateProvider;
    private readonly IFitTechApiClient _fitTechApiClient;
    private readonly ILogger<UserService> _logger;
    private readonly IStorage _storage;


    public UserService(IFitTechApiClient fitTechApiClient, FitTechAuthStateProvider authStateProvider,
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
            var result = await _fitTechApiClient.LoginAsync(new LoginRequest { Email = email, Password = password },
                cancellationToken);

            if (!result.Succeeded)
            {
                return result.MapFailure<FitTechUser>();
            }

            var user = new FitTechUser
            {
                Email = email, AccessToken = result.Value?.AccessToken, RefreshToken = result.Value?.RefreshToken
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

    public async Task<Result> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var result = await _fitTechApiClient.RegisterAsync(
            new RegisterRequest { Email = email, Password = password }, cancellationToken);

        return result.Succeeded
            ? Result.Success
            : Result.Failure(result.Errors.Select(x => x).ToArray());
    }

    public async Task<Result<string>> ForgotPasswordAsync(string to, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        var result = await _fitTechApiClient.ForgotPasswordAsync(
            new ForgotPasswordRequest
            {
                Email = to, CallbackUrl = "NotNeededRightNow" //TODO: Add redirect url
            }, cancellationToken);
        return new Result<string>
        {
            Errors = result.Errors.ToArray(), Succeeded = result.Succeeded, Value = result.Value
        };
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(newPassword);

        var result = await _fitTechApiClient.ResetPasswordAsync(
            new ResetPasswordRequest { Email = email, Token = token, NewPassword = newPassword }, cancellationToken);

        return result;
    }

    public async Task<Result> LogoutAsync(CancellationToken cancellationToken)
    {
        await _storage.ClearAsync(cancellationToken);
        return Result.Success;
    }

    public async Task<Result> AddClientAsync(string username, string lastname, DateTimeOffset birthdate, string email,
        int? phoneNumber, int? trainingHours, string trainingMode, string center, DateTimeOffset eventDate,
        string subscriptionType, CancellationToken cancellationToken)
    {
        
        var result = await _fitTechApiClient.AddNewClientAsync(
            new AddClientRequest
            {
                Name = username,
                LastName = lastname,
                Email = email,
                Birthdate = birthdate,
                TrainingHours = trainingHours,
                TrainingModel = trainingMode,
                EventDate = eventDate,
                Center = center,
                SubscriptionType = subscriptionType
            }, cancellationToken);
        
        return result;
    }
    
    
    public async Task<Result<TrainerDataDto>> GetTrainerDataAsync(CancellationToken cancellationToken)
    {
        var result = await _fitTechApiClient.GetTrainerDataAsync(cancellationToken);
        
        if (!result.Succeeded)
        {
            return result.MapFailure<TrainerDataDto>();
        }

        return result;
    }

    public async Task<Result> SaveChangesConfiguration(string name, string email, string password, CancellationToken cancellationToken)
    {
        var result = await  _fitTechApiClient.UpdateUserConfigurationAsync(
            new UpdateUSerConfigurationRequest()
            {
                Name = name,
                Email = email,
                Password = password,
                
            }, cancellationToken);

        return Result.Success;
    }

    public async Task<Result<ICollection<ClientDataDto>>> GetClientsDataAsync(
        CancellationToken cancellationToken)
    {
        var result = await _fitTechApiClient.GetClients(cancellationToken);

        if (!result.Succeeded)
        {
            return result.MapFailure<ICollection<ClientDataDto>>();
        }

        var clients = result.Value!; 
        
        return Result<ICollection<ClientDataDto>>.Success(clients);
    }
    
}
