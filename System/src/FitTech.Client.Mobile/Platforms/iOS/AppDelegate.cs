using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace FitTech.Client.Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp().GetAwaiter().GetResult();
    }
}
