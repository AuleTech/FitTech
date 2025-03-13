using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder
    .AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("fittechdb");

var fitTechApi = builder.AddProject<FitTech_API>("fittech-api")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithExternalHttpEndpoints();


_ = builder.AddProject<FitTech_Trainer_WebApp>("trainer-web")
    .WaitFor(fitTechApi)
    .WithEnvironment("FitTechApi__url", fitTechApi.GetEndpoint("https"));


builder.Build().Run();
