using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FitTech.WebComponents.Catalog;
using FitTech.WebComponents.Components.AppHeader;
using FitTech.WebComponents.Components.Snackbar;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<AppHeaderStateHandler>();
builder.Services.AddSingleton<IFitTechSnackbarService, FitTechSnackbarService>();

await builder.Build().RunAsync();
