using AuleTech.Core.Extensions.Language;
using FitTech.API.Client.Client;
using FitTech.API.Client.Client.Paths;
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
        public IServiceCollection AddFitTechApiClient(IConfiguration configuration)
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

            var httpClientBuilder = serviceCollection.AddHttpClient(nameof(IFitTechApiClient), client =>
            {
                client.BaseAddress = new Uri(fitTechConfig.Url);
            });
            
            httpClientBuilder.AddHttpMessageHandler<AuthDelegationHandler>();

            serviceCollection.AddRefitClient<ITrainerApiClient>(null, httpClientName: httpClientBuilder.Name);
            serviceCollection.AddRefitClient<IClientApiClient>(null, httpClientName: httpClientBuilder.Name);
            serviceCollection.AddRefitClient<IAuthenticationApiClient>();

            serviceCollection.AddTransient<IFitTechApiClient, FitTechApiClient>();
            
            return serviceCollection;
        }
    }
}
