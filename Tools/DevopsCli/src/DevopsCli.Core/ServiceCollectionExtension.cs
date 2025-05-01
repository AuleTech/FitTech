using AuleTech.Core;
using Cocona;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Sample;
using Microsoft.Extensions.DependencyInjection;

namespace DevopsCli.Core;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services
            .AddTransient<ICommand<SampleCommandParams, CommandResult>, SampleCommand>()
            .AddAuleTechPlatformCore()
            .Configure<CoconaAppOptions>(options =>
            {
                options.TreatPublicMethodsAsCommands = false;
            });
    }

    public static CoconaApp DiscoverAndWireUpCoconaCommands(this CoconaApp app)
    {
        var commandsTypes = typeof(ServiceCollectionExtension).Assembly.GetTypes()
            .Where(x => !x.IsInterface &&
                        x.GetInterfaces()?.FirstOrDefault(y => y.IsGenericType)?.GetGenericTypeDefinition() ==
                        typeof(ICommand<,>));

        app.AddCommands(commandsTypes);

        return app;
    }
}
