namespace FitTech.API.Client;

public interface ITokenStorage
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken);
    Task<string?> GetRefreshToken(CancellationToken cancellationToken);
    Task SetTokenAsync(string token, CancellationToken cancellationToken);
}
