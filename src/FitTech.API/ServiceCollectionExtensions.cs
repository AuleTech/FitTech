using System.Text;
using FitTech.API.Configuration;
using FitTech.Domain.Entities;
using FitTech.Persistence;
using Microsoft.IdentityModel.Tokens;

namespace FitTech.API;

internal static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddFitTechAuth(this WebApplicationBuilder builder)
    {
        var authSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();

        ArgumentNullException.ThrowIfNull(authSettings);

        builder.Services.AddIdentity<FitTechUser, FitTechRole>(options =>
        {
            options.Password.RequiredLength = 8;
        }).AddEntityFrameworkStores<FitTechDbContext>();

        builder.Services.AddAuthentication().AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = authSettings.Issuer,
                ValidAudience = authSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SigningKey))
            };
        });

        builder.Services.AddAuthorization();

        return builder;
    }
}
