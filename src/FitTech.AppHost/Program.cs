using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder
    .AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("fittechdb");

_ = builder.AddProject<FitTech_API>("fittech-api")
    .WithReference(postgresdb)
    .WithExternalHttpEndpoints();

builder.Build().Run();
