using System.Net.Http.Headers;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FitTech.API.Client;
using FitTech.API.Client.Client;
using FitTech.API.Client.Configuration;
using FitTech.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace FitTech.IntegrationTests;

public class TestHost : IAsyncInitializer, IAsyncDisposable
{
    private DistributedApplication? _app;
    public IFitTechApiClient ApiClient = null!;
    
    public async Task<FitTechDbContext> GetFitTechApiDbContextAsync(CancellationToken cancellationToken)
    {
        var connectionString = await _app!.GetConnectionStringAsync("fittechdb", cancellationToken);
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        dbContextOptionsBuilder.UseNpgsql(connectionString);

        return new FitTechDbContext(dbContextOptionsBuilder.Options);
    }

    public IServiceProvider GetServiceProvider() => _app!.Services;
    
    public async Task InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FitTech_AppHost>(["--environment=Testing","DOTNET_LAUNCH_PROFILE=http"]);
        
        _app = await builder.BuildAsync();
        await _app.StartAsync();
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("fittech-api");

        var client = _app.CreateHttpClient("fittech-api");

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string?>($"{FitTechApiConfiguration.ConfigSectionName}:Url",
                client.BaseAddress!.AbsoluteUri)
        }).Build();
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ITokenStorage, InMemoryTokenStorage>();
        
        serviceCollection.AddFitTechApiClient(configuration);
        ApiClient = serviceCollection.BuildServiceProvider().GetRequiredService<IFitTechApiClient>();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_app is null)
        {
            await _app!.DisposeAsync();
        }
    }
}
