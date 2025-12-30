using Blazored.LocalStorage;
using FitTech.Trainer.Wasm;
using FitTech.Trainer.Wasm.Persistence;
using FitTech.Trainer.Wasm.Services;
using FitTech.WebComponents;
using FitTech.WebComponents.Persistence;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services
    .AddBlazoredLocalStorage()
    .AddScoped<IStorage, BlazorLocalStorage>()
    .AddTransient<IInvitationService, InvitationService>()
    .AddFitTechComponents(builder.Configuration);
await builder.Build().RunAsync();
