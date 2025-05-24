using AuleTech.Core.Patterns;
using FitTech.API.Client.Configuration;
using FitTech.ApiClient;
using Result = AuleTech.Core.Patterns.Result;

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
        var result = await _proxy.LoginEndpointAsync(loginRequest, cancellationToken);

        return new Result<LoginResponse>()
        {
            Succeeded = result.Succeeded!.Value,
            Value = result.Value,
            Errors = result.Errors?.ToArray() ?? [] 
        };
    }

    public async Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var result = await _proxy.RegisterEndpointAsync(registerRequest, cancellationToken);

        return new Result()
        {
            Succeeded = result.Succeeded!.Value,
            Errors = result.Errors?.ToArray() ?? []
        };
    }

    public async Task<Result<string>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest,
        CancellationToken cancellationToken)
    {
        var result = await _proxy.RefreshTokenEndpointAsync(refreshTokenRequest, cancellationToken);

        return new Result<string>()
        {
            Succeeded = result.Succeeded!.Value, Errors = result.Errors?.ToArray() ?? [], Value = result.Value
        };
    }

    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest,
        CancellationToken cancellationToken)
    {
        var result = await _proxy.ForgotPasswordEndpointAsync(forgotPasswordRequest, cancellationToken);

        return new Result<string>()
        {
            Succeeded = result.Succeeded!.Value, Errors = result.Errors?.ToArray() ?? [], Value = result.Value
        };
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest,
        CancellationToken cancellationToken)
    {
        var result = await _proxy.ResetPasswordEndpointAsync(resetPasswordRequest, cancellationToken);

        return new Result() { Succeeded = result.Succeeded!.Value, Errors = result.Errors!.ToArray() ?? [] };
    }

    public async Task<Result<UserInfoDto>> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var result = await _proxy.GetCurrentUserEndpointAsync(cancellationToken);

        return new Result<UserInfoDto>() { Errors = result.Errors?.ToArray() ?? [], Succeeded = result.Succeeded!.Value };
    }
    
}
