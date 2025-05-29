using System.Text;
using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Providers;
using FitTech.Application.Auth.Services;
using FitTech.Application.Services;
using FitTech.Persistence.Repositories;
using FitTech.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Resend;

namespace FitTech.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
        
        ArgumentNullException.ThrowIfNull(authSettings);
        
        services.AddSingleton(authSettings);
        
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = authSettings.RefreshTokenExpirationTime;
        });
        
        services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
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
        services.AddTransient<IUserService, UserService>();
        
        services.AddAuthorization();
        
        services.AddEmailService(configuration);
        
        return services;
    }
    
    public static IServiceCollection AddEmailService(this IServiceCollection services , IConfiguration configuration)
    {
        //TODO: Extended to be part of ResendClientOptions -> ResendSettings : ResendClientOptions
        var secretsSettings = configuration
            .GetSection("SecretsSettings")
            .Get<SecretsSettings>();
        
        ArgumentNullException.ThrowIfNull(secretsSettings);
        
        services.AddSingleton(secretsSettings);

        
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(configuration.GetSection("Resend"));
        services.AddTransient<IResend, ResendClient>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IAddClientService, AddClientService>();
        services.AddTransient<IAddClientRepository, AddClientRepository>();
        services.AddTransient<AddClientService>();
        return services;
    }
}
