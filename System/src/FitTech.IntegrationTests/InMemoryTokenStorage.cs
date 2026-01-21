using FitTech.API.Client;
using FitTech.Application.Providers;

namespace FitTech.IntegrationTests;

internal class InMemoryTokenStorage : ITokenStorage
{
    private string? _token = null;
    private string? _refreshToken = null;

    public Task<string?> GetTokenAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_token);
    }

    public Task<string?> GetRefreshToken(CancellationToken cancellationToken)
    {
        return Task.FromResult(_refreshToken);  
    } 

    public Task SetTokenAsync(string token, CancellationToken cancellationToken)
    {
        _token = token;
        return Task.CompletedTask;
    }

    internal void SetRefreshToken(string refreshToken) => _refreshToken = refreshToken;
}
