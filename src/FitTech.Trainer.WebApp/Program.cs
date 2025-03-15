using FitTech.Trainer.WebApp.Components;
using FitTech.WebComponents;
using FitTech.WebComponents.Pages.Login;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddFitTechComponents(builder.Configuration)
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(Login).Assembly)
    .AddInteractiveServerRenderMode();

app.Run();
