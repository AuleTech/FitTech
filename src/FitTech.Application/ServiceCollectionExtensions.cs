using System.Text;
using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Providers;
using FitTech.Application.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FitTech.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
        
        ArgumentNullException.ThrowIfNull(authSettings);
        
        services.AddSingleton(authSettings!);
        
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = authSettings.RefreshTokenExpirationTime;
        });
        
        services.AddAuthentication().AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = authSettings.Issuer,
                ValidAudience = authSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SigningKey))
            };
        });

        services.AddTransient<IFitTechAuthenticationService, FitTechAuthenticationService>();
        services.AddTransient<ITokenProvider, TokenProvider>();
        
        services.AddAuthorization();
        
        return services;
    }
}
