using Blazored.LocalStorage;
using FitTech.API.Client;
using FitTech.WebComponents.Authentication;
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
        return serviceCollection
            .AddFitTechApiClient(configuration)
            .AddBlazoredLocalStorage()
            .AddTransient<IUserService, UserService>()
            .AddScoped<AuthenticationStateProvider, FitTechAuthStateProvider>()
            .AddScoped<FitTechAuthStateProvider>()
            .AddAuthorizationCore();
    }
}
