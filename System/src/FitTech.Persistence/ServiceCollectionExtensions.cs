﻿using FitTech.Domain.Interfaces;
using FitTech.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitTech.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection, string? connectionString)
    {
        serviceCollection.AddRepositories();

        serviceCollection.AddDbContext<FitTechDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
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

    private static IServiceCollection AddRepositories(this IServiceCollection service) =>
        service.AddTransient<IEmailRepository, EmailRepository>();
    
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var serviceScope = serviceProvider.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<FitTechDbContext>();

        await context.Database.EnsureCreatedAsync(cts.Token);
    }
}
