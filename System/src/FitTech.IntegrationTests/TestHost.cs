using Aspire.Hosting;
using Aspire.Hosting.Testing;
using TUnit.Core.Interfaces;

namespace FitTech.IntegrationTests;

public class TestHost : IAsyncInitializer, IAsyncDisposable
{
    private DistributedApplication? _app;

    public HttpClient GetClient(string name = "fittech-api")
    {
        return _app!.CreateHttpClient(name);
    }

    public IServiceProvider GetServiceProvider() => _app!.Services;
    
    public async Task InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FitTech_AppHost>();

        var app = await builder.BuildAsync();

        await app.StartAsync();
        await app.ResourceNotifications.WaitForResourceHealthyAsync("fittech-api");
        
        _app = app;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_app is null)
        {
            await _app!.DisposeAsync();
        }
    }
}
