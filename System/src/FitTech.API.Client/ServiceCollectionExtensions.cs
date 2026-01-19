using AuleTech.Core.Extensions.Language;
using FitTech.API.Client.ClientV2;
using FitTech.API.Client.ClientV2.Paths;
using FitTech.API.Client.Configuration;
using FitTech.ApiClient;
using FitTech.ApiClient.Generated;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace FitTech.API.Client;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddFitTechApiClient(IConfiguration configuration, Func<IServiceProvider, DelegatingHandler>? httpClientHandlerFunc = null)
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

        public IServiceCollection AddFitTechApiClientV2(IConfiguration configuration)
        {
            var fitTechConfig = configuration.GetSection(FitTechApiConfiguration.ConfigSectionName)
                .Get<FitTechApiConfiguration>();

            ArgumentNullException.ThrowIfNull(fitTechConfig);

            serviceCollection.AddSingleton(fitTechConfig);
            
            if (!serviceCollection.IsRegistered<ITokenStorage>())
            {
                throw new InvalidOperationException($"You need to implement and register {nameof(ITokenStorage)}");
            }

            serviceCollection.AddTransient<AuthDelegationHandler>();

            var httpClientBuilder = serviceCollection.AddHttpClient(nameof(IFitTechApiClientV2));
            httpClientBuilder.AddHttpMessageHandler<AuthDelegationHandler>();

            serviceCollection.AddRefitClient<ITrainerApiClient>(null, httpClientName: httpClientBuilder.Name);
            serviceCollection.AddRefitClient<IClientApiClient>(null, httpClientName: httpClientBuilder.Name);
            serviceCollection.AddRefitClient<IAuthenticationApiClient>(null, httpClientName: httpClientBuilder.Name);

            serviceCollection.AddTransient<IFitTechApiClientV2, FitTechApiClientV2>();
            
            return serviceCollection;
        }
    }
}
