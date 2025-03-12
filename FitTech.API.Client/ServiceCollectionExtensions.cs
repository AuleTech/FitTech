using FitTech.API.Client.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FitTech.API.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFitTechApiClient(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddHttpClient()
            .AddTransient<IFitTechApiClient, FitTechApiFitTechApiClient>();

        return serviceCollection;
    }
}
