using System.Text;
using FitTech.Application;
using FitTech.Application.Auth.Configuration;
using FitTech.Domain.Entities;
using FitTech.Persistence;
using Microsoft.IdentityModel.Tokens;

namespace FitTech.API;

internal static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddFitTechAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuth(builder.Configuration);
        
        builder.Services.AddIdentity<FitTechUser, FitTechRole>(options =>
        {
            options.Password.RequiredLength = 8;
        }).AddEntityFrameworkStores<FitTechDbContext>();

        return builder;
    }
}
