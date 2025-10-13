using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FitTech.Persistence;
using Microsoft.EntityFrameworkCore;
using TUnit.Core.Interfaces;

namespace FitTech.IntegrationTests;

public class TestHost : IAsyncInitializer, IAsyncDisposable
{
    private DistributedApplication? _app;

    public HttpClient GetClient(string name = "fittech-api")
    {
        return _app!.CreateHttpClient(name);
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
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FitTech_AppHost>(["--environment=Testing"]);

        var resource = builder.Resources.First(x => x.Name == "fittech-api");
        
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
