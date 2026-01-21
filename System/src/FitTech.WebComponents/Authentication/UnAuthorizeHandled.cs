using FitTech.API.Client;
using Microsoft.AspNetCore.Components;

namespace FitTech.WebComponents.Authentication;

internal class UnAuthorizeHandler : IUnauthorizeHandler
{
    private readonly NavigationManager _navigationManager;

    public UnAuthorizeHandler(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public Task OnAuthorizationFailedAsync()
    {
        _navigationManager.NavigateTo("/logout", true);

        return Task.CompletedTask;
    }
}
