using System.Text;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using AuleTech.Core.System.Host;
using FitTech.Application.Commands.Auth.ForgotPassword;
using FitTech.Application.Commands.Auth.Login;
using FitTech.Application.Commands.Auth.ResetPassword;
using FitTech.Application.Commands.Trainer.Register;
using FitTech.Application.Configuration;
using FitTech.Application.Providers;
using FitTech.Application.Query.Auth.RefreshToken;
using FitTech.Application.Query.Trainer.GetTrainerData;
using FitTech.Application.Services;
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

        services.AddTransient<ITokenProvider, TokenProvider>();
        services.AddTransient<IUserService, UserService>();
        services.RegisterAfterStartupJobs(typeof(ServiceCollectionExtensions).Assembly);

        services.AddAuthorization();

        services.AddEmailService(configuration);

        return services;
    }

    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        //TODO: Extended to be part of ResendClientOptions -> ResendSettings : ResendClientOptions
        var resendSettings = configuration
            .GetSection("ResendSettings")
            .Get<ResendSettings>();

        ArgumentNullException.ThrowIfNull(resendSettings);
        
        services.AddSingleton(resendSettings!);
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(configuration.GetSection("ResendSettings"));
        services.AddTransient<IResend, ResendClient>();
        services.AddTransient<IEmailService, EmailService>();
        
        return services;
    }

    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        return services.AddCommands()
            .AddQueries();
    }

    internal static IServiceCollection AddCommands(this IServiceCollection services)
    {
        return services
            .AddTransient<IForgotPasswordCommandHandler, ForgotPasswordCommandHandler>()
            .AddTransient<ILoginCommandHandler, LoginCommandHandler>()
            .AddTransient<IResetPasswordCommandHandler, ResetPasswordCommandHandler>()
            .AddTransient<IRegisterTrainerCommandHandler, RegisterTrainerCommandHandler>();
    }

    internal static IServiceCollection AddQueries(this IServiceCollection services)
    {
        return services
            .AddTransient<IRefreshTokenQueryHandler, RefreshTokenQueryHandler>()
            .AddTransient<IGetTrainerDataQueryHandler, GetTrainerDataQueryHandler>();
    }
}
