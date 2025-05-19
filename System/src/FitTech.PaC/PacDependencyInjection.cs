using DevopsCli.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class PacDependencyInjection
{ 
    private readonly IServiceProvider _serviceProvider;
    public static PacDependencyInjection Default = new ();
    
    private PacDependencyInjection()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(x => x.AddConsole().AddSimpleConsole()).AddCliCore();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
    
    public T Get<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
}
