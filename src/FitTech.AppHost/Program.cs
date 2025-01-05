
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("ankane/pgvector")
    .WithImageTag("latest")
    .WithLifetime(ContainerLifetime.Persistent);

var identityDb = postgres.AddDatabase("identitydb");
var identityApi = builder.AddProject<Projects.FitTech_Identity_API>("identity-api")
    .WithExternalHttpEndpoints()
    .WithReference(identityDb);

var identityEndpoint = identityApi.GetEndpoint("https");

var webApp = builder.AddProject<Projects.FitTech_Trainers_WebApp>("trainers-webapp").WithExternalHttpEndpoints().WithEnvironment("IdentityUrl", identityEndpoint);

webApp.WithEnvironment("CallbackUrl", webApp.GetEndpoint("https"));
identityApi.WithEnvironment("TrainerWebApp", webApp.GetEndpoint("https"));
builder.Build().Run();
