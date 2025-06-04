using System.Reflection;
using FitTech.Client.Mobile.Persistence;
using FitTech.WebComponents;
using FitTech.WebComponents.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FitTech.Client.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureEssentials()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        
        builder.Services.AddMauiBlazorWebView();
        builder.AddAppSettings();
        builder.Services
            .AddScoped<IStorage, MauiStorage>()
            .AddSingleton<IPreferences>(_ => Preferences.Default)
            .AddFitTechComponents(builder.Configuration);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void AddAppSettings(this MauiAppBuilder builder)
    {
        using Stream appsettings = GetAppSettingsFile()!;
        using Stream environmentAppSettings = GetAppSettingsFile(Environments.Development)!; //TODO: Define how to set environment
        
        builder.Configuration
            .AddJsonStream(appsettings)
            .AddJsonStream(environmentAppSettings);

        Stream? GetAppSettingsFile(string? environment = null)
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(string.IsNullOrWhiteSpace(environment)
                    ? "FitTech.Client.Mobile.appsettings.json"
                    : $"FitTech.Client.Mobile.appsettings.{environment}.json");
        }
    }
}
