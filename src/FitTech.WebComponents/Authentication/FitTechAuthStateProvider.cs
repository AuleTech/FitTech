using System.Security.Claims;
using Blazored.LocalStorage;
using FitTech.WebComponents.Models;
using Microsoft.AspNetCore.Components.Authorization;
namespace FitTech.WebComponents.Authentication;

internal sealed class FitTechAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    public FitTechAuthStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _localStorageService.GetItemAsync<FitTechUser>(FitTechUser.StorageKey);

        return new AuthenticationState(user is not null ? user.GetClaimsPrincipal() : new ClaimsPrincipal());
    }

    public void RaiseLoginEvent(FitTechUser user)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user.GetClaimsPrincipal())));
    }
}
