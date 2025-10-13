using FitTech.Application;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Persistence;
using Microsoft.AspNetCore.Identity;

namespace FitTech.API;

internal static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddFitTechAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuth(builder.Configuration);

        builder.Services.AddIdentityCore<FitTechUser>(options =>
            {
                options.Password.RequiredLength = 8;
            })
            .AddRoles<FitTechRole>()
            .AddEntityFrameworkStores<FitTechDbContext>()
            .AddDefaultTokenProviders();

        return builder;
    }
}
