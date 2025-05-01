// See https://aka.ms/new-console-template for more information

using Cocona;
using DevopsCli.Core;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddCore();

var app = builder.Build();
app.DiscoverAndWireUpCoconaCommands();
await app.RunAsync();
