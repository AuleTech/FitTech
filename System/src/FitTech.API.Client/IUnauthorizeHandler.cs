namespace FitTech.API.Client;

public interface IUnauthorizeHandler
{
    Task OnAuthorizationFailedAsync();
}
