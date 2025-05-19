using AuleTech.Core.Processing.Runners;
using AuleTech.Core.Processing.Runners.Factory;
using AuleTech.Core.System.IO.FileSystem;
using AuleTech.Core.System.IO.FileSystem.Compression;
using AuleTech.Core.System.IO.FileSystem.Directories;
using AuleTech.Core.System.IO.FileSystem.Files;
using Microsoft.Extensions.DependencyInjection;

namespace AuleTech.Core;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddAuleTechPlatformCore(this IServiceCollection container)
    {
        return container.AddIo().AddProcessing();
    }

    private static IServiceCollection AddIo(this IServiceCollection container)
    {
        return container
            .AddTransient<ISystemIo, SystemIoProxy>()
            .AddTransient<ISystemIoPath, SystemIoPathProxy>()
            .AddTransient<ISystemIoDirectory, SystemIoDirectoryProxy>()
            .AddTransient<ISystemIoFile>(c => new SystemIoFileProxy(false))
            .AddTransient<ISystemIoFileDirect>(c => new SystemIoFileProxy(true))
            .AddTransient<ISystemIOCompression, SystemIoCompressionProxy>();
    }

    private static IServiceCollection AddProcessing(this IServiceCollection container)
    {
        return container
            .AddTransient<IProcessRunner, CommandLineProcessRunner>()
            .AddTransient<IProcessRunnerFactory, CommandLineProcessRunnerFactory>();
    }
}
