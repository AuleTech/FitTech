using System.Reflection;
using AuleTech.Core;
using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using AuleTech.Core.Reflection;
using Cocona;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Dotnet.Build;
using DevopsCli.Core.Commands.Dotnet.Restore;
using DevopsCli.Core.Commands.Dotnet.Test;
using DevopsCli.Core.Commands.Dotnet.Workloads;
using DevopsCli.Core.Commands.GenerateOpenApiTypedClient;
using DevopsCli.Core.Commands.Sample;
using DevopsCli.Core.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace DevopsCli.Core;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCliCore(this IServiceCollection services)
    {
        return services
            .RegisterCommands()
            .DiscoverAndRegisterTools()
            .AddAuleTechPlatformCore()
            .Configure<CoconaAppOptions>(options =>
            {
                options.TreatPublicMethodsAsCommands = false;
            });
    }

    public static CoconaApp DiscoverAndWireUpCoconaCommands(this CoconaApp app)
    {
        var commandsTypes = typeof(ServiceCollectionExtension).Assembly.GetTypes()
            .Where(x => !x.IsInterface && !x.IsAbstract &&
                        x.GetInterfaces()?.FirstOrDefault(y => y.IsGenericType)?.GetGenericTypeDefinition() ==
                        typeof(ICommand<,>));

        app.AddCommands(commandsTypes);

        return app;
    }

    public static IServiceCollection RegisterCommands(this IServiceCollection services) => services
        .AddTransient<ICommand<SampleCommandParams, Result>, SampleCommand>()
        .AddTransient<ICommand<GenerateOpenApiTypedClientParams, Result>, GenerateOpenApiTypedClientCommand>()
        .AddTransient<ICommand<RestoreCommandParams, Result>, RestoreCommand>()
        .AddTransient<ICommand<WorkloadsCommandParams, Result>, WorkloadsCommand>()
        .AddTransient<ICommand<RunTestsCommandParams, Result>, RunTestsCommand>()
        .AddTransient<ICommand<BuildCommandParams, Result>, BuildCommand>(); 

    public static IServiceCollection DiscoverAndRegisterTools(this IServiceCollection serviceCollection)
    {
        var currentAssemblyTypes = typeof(ServiceCollectionExtension).Assembly.GetTypes();

        var toolsType = currentAssemblyTypes.Where(x =>
            x is { IsInterface: false, IsAbstract: false } && typeof(ITool).IsAssignableFrom(x));

        RegisterInstallers();

        foreach (var toolType in toolsType)
        {
            var implementedInterfaces = toolType.GetInterfaces();

            foreach (var toolInterface in implementedInterfaces)
            {
                serviceCollection.AddTransient(toolInterface, toolType);
            }

            var installerInterface = currentAssemblyTypes.SelectMany(x => x.GetInterfaces()).Distinct().Where(x =>
                    x.IsGenericType
                    && x.GetGenericTypeDefinition().Name.Contains("IInstaller")
                    && x.GetGenericArguments()[0] == toolType)
                .ToArray();

            if (!installerInterface.Any())
            {
                continue;
            }

            if (installerInterface.Count() > 1)
            {
                throw new InvalidOperationException(
                    $"There are multiple installers interfaces for the same tool [{string.Join(",", installerInterface.Select(x => x.Name))}]");
            }

            var installerInterfaceSingle = installerInterface.Single();
            
            serviceCollection.AddTransient(installerInterfaceSingle, sp =>
            {
                var implementationTypes = currentAssemblyTypes.Where(x =>
                    !x.IsInterface && !x.IsAbstract && installerInterfaceSingle.IsAssignableFrom(x));

                var isPlatformSupportedMethod = installerInterfaceSingle.GetMethod("IsSupported")!;
                
                foreach (var implementationType in implementationTypes)
                {
                    var installer = sp.GetService(implementationType)!;
                    var isSupported = (bool)isPlatformSupportedMethod.Invoke(installer, [Environment.OSVersion.Platform])!;

                    if (isSupported)
                    {
                        return installer;
                    }
                }

                throw new InvalidOperationException(
                    $"Not supported installer found for {toolType.Name} in current OS('{Environment.OSVersion.Platform}')");
            });
        }

        return serviceCollection;
        
        void RegisterInstallers()
        {
            var simpleInstallers = currentAssemblyTypes.Where(x => x.ImplementsGenericInterface(typeof(IInstaller<>)));
            var configurableInstallers = currentAssemblyTypes.Where(x => x.ImplementsGenericInterface(typeof(IInstaller<,>)));

            foreach (var simpleInstaller in simpleInstallers)
            {
                serviceCollection.AddTransient(simpleInstaller);
            }

            foreach (var configurableInstaller in configurableInstallers)
            {
                serviceCollection.AddTransient(configurableInstaller);
            }
        }
    }
}
