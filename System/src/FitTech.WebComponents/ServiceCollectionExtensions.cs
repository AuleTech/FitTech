using FitTech.API.Client;
using FitTech.WebComponents.Authentication;
using FitTech.WebComponents.Components.AppHeader;
using FitTech.WebComponents.Components.Snackbar;
using FitTech.WebComponents.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitTech.WebComponents;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFitTechComponents(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
       // serviceCollection.AddScoped<FitTechDelegationHandler>();
        serviceCollection.AddSingleton<AppHeaderStateHandler>();
        serviceCollection.AddTransient<ITokenStorage, TokenStorage>();
        serviceCollection.AddTransient<IUnauthorizeHandler, UnAuthorizeHandler>();
        
        return serviceCollection
            .AddFitTechApiClient(configuration)
            .AddTransient<IUserService, UserService>()
            .AddSingleton<IFitTechSnackbarService, FitTechSnackbarService>()
            .AddScoped<AuthenticationStateProvider, FitTechAuthStateProvider>()
            .AddScoped<FitTechAuthStateProvider>()
            .AddAuthorizationCore();
    }
}
