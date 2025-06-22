using System.Reflection;
using AuleTech.Core.System.Host;
using FitTech.Application;
using FitTech.Domain.Entities;
using FitTech.Persistence;
using Microsoft.AspNetCore.Identity;

namespace FitTech.API;

internal static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddFitTechAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuth(builder.Configuration);

        builder.Services.AddIdentity<FitTechUser, FitTechRole>(options =>
            {
                options.Password.RequiredLength = 8;
            })
            .AddRoles<FitTechRole>()
            .AddEntityFrameworkStores<FitTechDbContext>()
            .AddDefaultTokenProviders();

        return builder;
    }
}
