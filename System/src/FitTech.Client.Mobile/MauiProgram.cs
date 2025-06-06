using System.Reflection;
using System.Text;
using AuleTech.Core.Maui;
using AuleTech.Core.System.IO;
using FitTech.Client.Mobile.Persistence;
using FitTech.WebComponents;
using FitTech.WebComponents.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            .AddFitTechComponents(builder.Configuration);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
