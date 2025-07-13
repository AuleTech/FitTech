using AuleTech.Core.Messaging;
using AuleTech.Core.Messaging.Rabbit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Assembly = System.Reflection.Assembly;

namespace AuleTech.Core.Message.IntegrationTests.Rabbit;

public class RabbitMessagingTests
{
    private static IServiceProvider? _serviceProvider;
    
    [Before(Class)]
    public static void Setup()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection([
            new KeyValuePair<string, string?>(
                $"{RabbitMqConfiguration.SectionName}:{nameof(RabbitMqConfiguration.ConnectionString)}", "something")
        ]).Build();
        
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddQueues(configuration, typeof(RabbitMessagingTests).Assembly);
        serviceCollection.AddLogging();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        _serviceProvider = serviceProvider;
    }

    [Test]
    [Timeout(30_000)]
    public async Task CanPublishAndConsumer(CancellationToken cancellationToken)
    {
        
    }
}
