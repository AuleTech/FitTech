using FitTech.API.Client;
using FitTech.WebComponents.Models;
using FitTech.WebComponents.Persistence;

namespace FitTech.WebComponents.Authentication;

public class TokenStorage : ITokenStorage
{
    private readonly IStorage _storage;

    public TokenStorage(IStorage storage)
    {
        _storage = storage;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken) =>
        (await _storage.GetItemAsync<FitTechUser>(FitTechUser.StorageKey, cancellationToken))?.AccessToken;

    public async Task<string?> GetRefreshToken(CancellationToken cancellationToken) => 
        (await _storage.GetItemAsync<FitTechUser>(FitTechUser.StorageKey, cancellationToken))?.RefreshToken;

    public async Task SetTokenAsync(string token, CancellationToken cancellationToken)
    {
        var user = await _storage.GetItemAsync<FitTechUser>(FitTechUser.StorageKey, cancellationToken);

        if (user is null)
        {
            return;
        }
        
        user.AccessToken = token;

        await _storage.SetItemAsync(FitTechUser.StorageKey, user, cancellationToken);
    }
}
