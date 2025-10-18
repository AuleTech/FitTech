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
        catch (Exception ex)
        {
            return Result<string>.Failure(ex.Message);
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
        catch (Exception ex)
        {
            return Result<string>.Failure(ex.Message);
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
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result<UserInfoDto>> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.GetCurrentUserEndpointAsync(cancellationToken);

            return Result<UserInfoDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<UserInfoDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<TrainerDataDto>> GetTrainerDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.GetTrainerSettingsEndpointAsync(cancellationToken);

            return Result<TrainerDataDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<TrainerDataDto>.Failure(ex.Message);
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
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result> SendInvitationAsync(InviteClientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _proxy.InviteClientEndpointAsync(request, cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result<GetInvitationsResponse>> GetInvitationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.GetInvitationsEndpointAsync(cancellationToken);
            return Result<GetInvitationsResponse>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<GetInvitationsResponse>.Failure(ex.Message);
        }
    }

    public async Task<Result> RegisterClientAsync(RegisterClientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _proxy.RegisterClientEndpointAsync(request, cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result<Guid>> ValidateInvitationAsync(string email, int code, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _proxy.ValidateInvitationEndpointAsync(code, email, cancellationToken);
            return Result<Guid>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
