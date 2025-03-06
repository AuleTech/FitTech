using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitTech.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection, string? connectionString)
    {
        serviceCollection.AddDbContext<FitTechDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return serviceCollection;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var serviceScope = serviceProvider.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<FitTechDbContext>();

        await context.Database.EnsureCreatedAsync(cts.Token);
    }
}
