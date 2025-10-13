using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AuleTech.Core.System.Host;

public static class HostExtensions
{
    public static async Task RunPostStartupActionsAsync(this IServiceProvider hostServiceProvider, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        await using var scope = hostServiceProvider.CreateAsyncScope();
        await scope.ServiceProvider.RunAfterStartupJobsAsync(cts.Token);
    }

    internal static async Task RunAfterStartupJobsAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var jobs = serviceProvider.GetRequiredService<IEnumerable<IAfterStartupJob>>();

        foreach (var job in jobs)
        {
            await job.RunAsync(cancellationToken);
        }
    }

    public static void RegisterAfterStartupJobs(this IServiceCollection serviceCollection, Assembly assembly)
    {
        foreach (var afterStartupJob in assembly.GetTypes().Where(x =>
                     x is { IsInterface: false, IsAbstract: false } && typeof(IAfterStartupJob).IsAssignableFrom(x)))
        {
            serviceCollection.AddTransient(typeof(IAfterStartupJob), afterStartupJob);
        }
    }
}
