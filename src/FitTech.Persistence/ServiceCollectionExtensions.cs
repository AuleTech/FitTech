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
}
