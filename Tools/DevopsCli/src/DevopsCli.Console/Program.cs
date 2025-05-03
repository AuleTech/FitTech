// See https://aka.ms/new-console-template for more information

using AuleTech.Core.Patterns;
using Cocona;
using DevopsCli.Core;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.GenerateOpenApiTypedClient;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddCore();

var app = builder.Build();
app.DiscoverAndWireUpCoconaCommands();
await app.RunAsync();
