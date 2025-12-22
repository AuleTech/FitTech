using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", "fittech2025");

var postgres = builder
    .AddPostgres("postgres", password: password, port: 5432);

var postgresdb = postgres.WithDataVolume().AddDatabase("fittechdb");
var rabbitMq = builder.AddRabbitMQ("messaging").WithManagementPlugin();

var fitTechApi = builder
    .AddProject<FitTech_API>("fittech-api")
    .WithReference(postgresdb)
    .WithReference(rabbitMq)
    .WaitFor(postgresdb)
    .WaitFor(rabbitMq)
    .WithExternalHttpEndpoints()
    .WithEnvironment("RabbitMq__ConnectionString", rabbitMq.Resource.ConnectionStringExpression)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

_ = builder.AddProject<FitTech_Trainer_Wasm>("trainer-web")
    .WaitFor(fitTechApi);

builder.Build().Run();
