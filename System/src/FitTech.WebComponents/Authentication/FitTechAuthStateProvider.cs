using System.Security.Claims;
using Blazored.LocalStorage;
using FitTech.WebComponents.Models;
using FitTech.WebComponents.Persistence;
using Microsoft.AspNetCore.Components.Authorization;
namespace FitTech.WebComponents.Authentication;

internal sealed class FitTechAuthStateProvider : AuthenticationStateProvider
{
    private readonly IStorage _storage;
    public FitTechAuthStateProvider(IStorage storage)
    {
        _storage = storage;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _storage.GetItemAsync<FitTechUser>(FitTechUser.StorageKey, CancellationToken.None);

        return new AuthenticationState(user is not null ? user.GetClaimsPrincipal() : new ClaimsPrincipal([]));
    }

    public void RaiseLoginEvent(FitTechUser user)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user.GetClaimsPrincipal())));
    }
}
