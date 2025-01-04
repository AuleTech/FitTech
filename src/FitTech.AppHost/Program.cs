using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<FitTech_Trainers_WebApp>("trainers-webapp");

builder.Build().Run();
