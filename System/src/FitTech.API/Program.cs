using FastEndpoints;
using FastEndpoints.Swagger;
using FitTech.API;
using FitTech.Persistence;
using Resend;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider((_, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

var connectionString = builder.Configuration.GetConnectionString("fittechdb");

builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(
    builder.Configuration.GetSection("Resend")
);

builder.Services.AddTransient<IResend, ResendClient>();

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
        policyBuilder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:7083");
    }))
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
    .UseCors("FitTechCorsPolicy")
    .UseAuthorization()
    .UseAuthentication()
    .UseFastEndpoints(x => x.Endpoints.ShortNames = true);

//TODO: Create a migration service triggered by Aspire
await app.Services.ApplyMigrationsAsync();

app.Run();
