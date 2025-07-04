using AuleTech.Core.System.Host;
using FastEndpoints;
using FastEndpoints.Swagger;
using FitTech.API;
using FitTech.Application;
using FitTech.Persistence;
using FitTech.Application.Services;
using FitTech.Persistence.Repositories;
    
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider((_, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

var connectionString = builder.Configuration.GetConnectionString("fittechdb");

builder.AddFitTechAuth();
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(x =>
    {
        x.ShortSchemaNames = true;
        x.DocumentSettings = d =>
        {
            d.Title = "FitTech.API";
            d.Version = "v1";
        };
    })
    .AddLogging()
    .AddOpenApi()
    .AddCors( c => c.AddPolicy("FitTechCorsPolicy", policyBuilder =>
    {
        policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();

    }))
    .AddPersistence(connectionString)
    .AddCQRS();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerGen();
}

app
    .UseHttpsRedirection()
    .UseCors("FitTechCorsPolicy")
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(x => x.Endpoints.ShortNames = true);

//TODO: Create a migration service triggered by Aspire
await app.Services.ApplyMigrationsAsync(); //TODO: Move to RunPostStartupActionsAsync
await app.Services.RunPostStartupActionsAsync(TimeSpan.FromSeconds(30));

app.Run();
