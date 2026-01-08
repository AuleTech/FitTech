using System.Reflection;
using System.Threading.Tasks;
using AuleTech.Core.Maui;
using FitTech.Client.Mobile.Persistence;
using FitTech.Client.Mobile.Services;
using FitTech.WebComponents;
using FitTech.WebComponents.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;

namespace FitTech.Client.Mobile;

public static class MauiProgram
{
    public static async Task<MauiApp> CreateMauiApp()
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
        await builder.AddAppSettingsAsync("FitTech.Client.Mobile", Assembly.GetExecutingAssembly());
        builder.Services
            .AddScoped<IStorage, MauiStorage>()
            .AddSingleton<IPreferences>(_ => Preferences.Default)
            .AddSingleton<IRegistrationService, RegistrationService>()
            .AddFitTechComponents(builder.Configuration);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
