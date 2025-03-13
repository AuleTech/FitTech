using FitTech.API.Client.Configuration;
using FitTech.API.Client.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitTech.API.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFitTechApiClient(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var fitTechConfig = configuration.GetSection(FitTechApiConfiguration.ConfigSectionName)
            .Get<FitTechApiConfiguration>();

        ArgumentNullException.ThrowIfNull(fitTechConfig);

        serviceCollection.AddSingleton(fitTechConfig);

        serviceCollection
            .AddHttpClient()
            .AddTransient<IFitTechApiClient>(c =>
            {
                var httpClientFactory = c.GetRequiredService<IHttpClientFactory>();

                var fitTechApiClient =
                    new FitTechApiFitTechApiClient(httpClientFactory.CreateClient(nameof(FitTechApiFitTechApiClient)))
                    {
                        BaseUrl = fitTechConfig.Url
                    };

                return fitTechApiClient;
            });

        return serviceCollection;
    }
}
