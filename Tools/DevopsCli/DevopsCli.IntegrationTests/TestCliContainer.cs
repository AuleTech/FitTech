using DevopsCli.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DevopsCli.IntegrationTests;

public class TestCliContainer : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return ServiceProvider.CreateScope();
    }

    public override object? Create(IServiceScope scope, Type type)
    {
        return scope.ServiceProvider.GetService(type);
    }
    
    private static IServiceProvider CreateSharedServiceProvider()
    {
        return new ServiceCollection()
            .AddCore()
            .AddLogging()
            .BuildServiceProvider();

    }
}
