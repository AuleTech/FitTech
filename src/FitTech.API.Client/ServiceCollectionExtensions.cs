using FitTech.API.Client.Configuration;
using FitTech.Api.Client.Generated;
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
            .AddHttpClient(nameof(FitTechAPIClient));

        if (httpClientHandlerFunc is not null)
        {
            apiHttpClientBuilder.AddHttpMessageHandler(httpClientHandlerFunc!);
        }


        serviceCollection.AddTransient<FitTechAPIClient>(c =>
        {
            var httpClientFactory = c.GetRequiredService<IHttpClientFactory>();

            var fitTechApiClient =
                new FitTechAPIClient(httpClientFactory.CreateClient(nameof(FitTechAPIClient)))
                {
                    BaseUrl = fitTechConfig.Url
                };


            return fitTechApiClient;
        });

        return serviceCollection;
    }
}
