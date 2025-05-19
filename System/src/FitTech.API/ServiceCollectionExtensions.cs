using FitTech.Application;
using FitTech.Application.Auth.Services;
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
        }).AddEntityFrameworkStores<FitTechDbContext>()
        .AddDefaultTokenProviders();

        return builder;
    }
}
