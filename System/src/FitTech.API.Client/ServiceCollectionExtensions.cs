using FitTech.API.Client.Configuration;
using FitTech.ApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitTech.API.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFitTechApiClient(this IServiceCollection serviceCollection,
        IConfiguration configuration, Func<IServiceProvider, DelegatingHandler>? httpClientHandlerFunc = null)
    {
        var fitTechConfig = configuration.GetSection(FitTechApiConfiguration.ConfigSectionName)
            .Get<FitTechApiConfiguration>();

        ArgumentNullException.ThrowIfNull(fitTechConfig);

        serviceCollection.AddSingleton(fitTechConfig);

        var apiHttpClientBuilder = serviceCollection
            .AddHttpClient(nameof(Proxy));

        if (httpClientHandlerFunc is not null)
        {
            apiHttpClientBuilder.AddHttpMessageHandler(httpClientHandlerFunc!);
        }
        
        serviceCollection.AddTransient<IFitTechApiClient, FitTechApiClient>();
        serviceCollection.AddTransient<IFitTechApiClientFactory, FitTechApiClientFactory>();

        return serviceCollection;
    }
}
