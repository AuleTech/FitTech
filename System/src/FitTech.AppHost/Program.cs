using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder
    .AddPostgres("postgres");

var postgresdb = postgres.AddDatabase("fittechdb");
var fitTechApi = builder.AddProject<FitTech_API>("fittech-api")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithExternalHttpEndpoints();

_ = builder.AddProject<FitTech_Trainer_Wasm>("trainer-web")
    .WaitFor(fitTechApi);

builder.Build().Run();
