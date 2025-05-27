using Blazored.LocalStorage;
using FitTech.API.Client;
using FitTech.WebComponents.Authentication;
using FitTech.WebComponents.Components.AppHeader;
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
        serviceCollection.AddScoped<FitTechDelegationHandler>();
        serviceCollection.AddSingleton<AppHeaderStateHandler>();
        
        return serviceCollection
            .AddFitTechApiClient(configuration, provider => provider.GetRequiredService<FitTechDelegationHandler>())
            .AddBlazoredLocalStorage() //TODO: Expose an interface to be implemented by the front to manage the user
            .AddTransient<IUserService, UserService>()
            .AddScoped<AuthenticationStateProvider, FitTechAuthStateProvider>()
            .AddScoped<FitTechAuthStateProvider>()
            .AddAuthorizationCore();
    }
}
