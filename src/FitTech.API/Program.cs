using FastEndpoints;
using FastEndpoints.Swagger;
using FitTech.API;
using FitTech.Persistence;

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
    .AddCors()
    .AddPersistence(connectionString);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerGen();
}

app
    .UseHttpsRedirection()
    .UseAuthorization()
    .UseAuthentication()
    .UseFastEndpoints(x => x.Endpoints.ShortNames = true)
    .UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

//TODO: Create a migration service triggered by Aspire
await app.Services.ApplyMigrationsAsync();

app.Run();
