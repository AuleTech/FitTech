using System.Reflection;
using AuleTech.Core.Messaging.Rabbit;
using AuleTech.Core.Messaging.Rabbit.Configuration;
using AuleTech.Core.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AuleTech.Core.Messaging;

public static class ServiceCollectionExtensions
{
    
    public static IServiceCollection AddQueues(this IServiceCollection services, IConfiguration configuration, Assembly consumersAssembly)
    {
        var rabbitMqConfiguration = configuration.GetSection(RabbitMqConfiguration.SectionName).Get<RabbitMqConfiguration>();

        if (rabbitMqConfiguration is not null)
        {
            services.AddRabbitMq(rabbitMqConfiguration, consumersAssembly);
        }
        
        return services;
    }

    private static IServiceCollection AddRabbitMq(this IServiceCollection services, RabbitMqConfiguration configuration ,Assembly consumersAssembly)
    {
        var consumers = consumersAssembly.GetTypes()
            .Where(x => x.ImplementsGenericInterface(typeof(IAuleTechConsumer<>))).ToArray();
        
        foreach (var consumer in consumers)
        {
            var consumerInterface = consumer.GetInterface(typeof(IAuleTechConsumer<>).Name)!;
            services.AddTransient(consumerInterface,consumer);

            services.AddSingleton(typeof(IAuleTechQueueListener),
                typeof(AuleTechRabbitQueueListener<>).MakeGenericType(consumerInterface.GetGenericArguments()[0]));
        }

        services.AddSingleton(configuration);
        services.AddTransient<IAuleTechQueuePublisher, RabbitQueuePublisher>();
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory()
        {
            Uri = new Uri(configuration.ConnectionString)
        });

        services.AddHostedService<AuleTechListenerInitializer>();
        
        return services;
    }
}
