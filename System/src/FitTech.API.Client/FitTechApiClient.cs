using AuleTech.Core.Patterns.Result;
using FitTech.API.Client.Configuration;
using FitTech.ApiClient;
using Result = AuleTech.Core.Patterns.Result.Result;

namespace FitTech.API.Client;

internal sealed class FitTechApiClient : IFitTechApiClient
{
    private readonly Proxy _proxy;

    public FitTechApiClient(FitTechApiConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        var httpClient = httpClientFactory.CreateClient(nameof(Proxy));
        httpClient.BaseAddress = new Uri(configuration.Url);

        _proxy = new Proxy(httpClient);
    }

    internal FitTechApiClient(HttpClient client)
    {
        _proxy = new Proxy(client);
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.LoginEndpointAsync(loginRequest, cancellationToken);

            return Result<LoginResponse>.Success(result);
        }
        catch (Exception)
        {
            return Result<LoginResponse>.Failure("Login failed");
        }
    }

    public async Task<Result<string>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.RefreshTokenEndpointAsync(refreshTokenRequest, cancellationToken);

            return Result<string>.Success(result);
        }
        catch (Exception)
        {
            return Result<string>.Failure("Couldn't not refresh token");
        }
    }

    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.ForgotPasswordEndpointAsync(forgotPasswordRequest, cancellationToken);

            return Result<string>.Success(result);
        }
        catch (Exception)
        {
            return Result<string>.Failure("Something went wrong");
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            await _proxy.ResetPasswordEndpointAsync(resetPasswordRequest, cancellationToken);

            return Result.Success;
        }
        catch (Exception)
        {
            return Result.Failure("Something went wrong");
        }
    }

    public async Task<Result<UserInfoDto>> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.GetCurrentUserEndpointAsync(cancellationToken);

            return Result<UserInfoDto>.Success(result);
        }
        catch (Exception)
        {
            return Result<UserInfoDto>.Failure("Something went wrong");
        }
    }

    public async Task<Result<TrainerDataDto>> GetTrainerDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.GetTrainerSettingsEndpointAsync(cancellationToken);

            return Result<TrainerDataDto>.Success(result);
        }
        catch (Exception)
        {
            return Result<TrainerDataDto>.Failure("Something went wrong");
        }
    }

    public async Task<Result> RegisterTrainerAsync(RegisterTrainerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _proxy.RegisterTrainerEndpointAsync(request, cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Result.Failure($"Something went wrong: ${ex.Message}");
        }
    }
}
