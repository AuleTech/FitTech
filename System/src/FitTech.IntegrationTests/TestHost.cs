using System.Net.Http.Headers;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FitTech.API.Client;
using FitTech.Persistence;
using Microsoft.EntityFrameworkCore;
using TUnit.Core.Interfaces;

namespace FitTech.IntegrationTests;

public class TestHost : IAsyncInitializer, IAsyncDisposable
{
    private DistributedApplication? _app;

    internal IFitTechApiClient GetClientApiClient(string? authenticationToken = null, string name = "fittech-api")
    {
        var client = _app!.CreateHttpClient(name);

        if (!string.IsNullOrWhiteSpace(authenticationToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationToken);
        }
        
        return new FitTechApiClient(client);
    }
    
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
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FitTech_AppHost>(["--environment=Testing","DOTNET_LAUNCH_PROFILE=http","ASPIRE_ALLOW_UNSECURED_TRANSPORT=true"],
            (options, settings) =>
            {
                options.DisableDashboard = false;
            });
        
        _app = await builder.BuildAsync();
        await _app.StartAsync();
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("fittech-api");
        
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_app is null)
        {
            await _app!.DisposeAsync();
        }
    }
}
