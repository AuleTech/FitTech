using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using FitTech.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitTech.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection, string? connectionString)
    {
        serviceCollection.AddRepositories();

        serviceCollection.AddDbContext<FitTechDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                if (context.Set<FitTechUser>().Any())
                {
                    return;
                }
                
                var passwordHasher = new PasswordHasher<FitTechUser>();

                var email = "admin@fittech.es";

                var trainer = Trainer.Create("Admin", "Admin", email);
                
                var fitTechAdminUser = new FitTechUser
                {
                    Id = trainer.Value!.Id,
                    UserName = email,
                    NormalizedUserName = email.ToUpperInvariant(),
                    Email = email,
                    NormalizedEmail = email.ToUpperInvariant(),
                    SecurityStamp = "PAXJBLPK3OCIXX4B62CQI4ECMLDCBCPN"
                };

                fitTechAdminUser.PasswordHash = passwordHasher.HashPassword(fitTechAdminUser, "FitTech2025!");

                await context.AddAsync(fitTechAdminUser, cancellationToken);
                await context.AddAsync(trainer.Value!, cancellationToken);
                
                await context.SaveChangesAsync(cancellationToken);
            });
        });

        return serviceCollection;
    }

    public static IServiceCollection AddInMemorydb(this IServiceCollection serviceCollection, string dbName)
    {
        serviceCollection.AddRepositories();

        serviceCollection.AddDbContext<FitTechDbContext>(options =>
        {
            options.UseInMemoryDatabase(dbName);
        });

        return serviceCollection;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection service)
    {
        service.AddScoped<IUnitOfWork, UnitOfWork>()
            .AddTransient<IEmailRepository, EmailRepository>()
            .AddTransient<ITrainerRepository, TrainerRepository>()
            .AddTransient<IClientRepository, ClientRepository>();

        return service;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var serviceScope = serviceProvider.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<FitTechDbContext>();

        await context.Database.EnsureCreatedAsync(cts.Token);
    }
}
