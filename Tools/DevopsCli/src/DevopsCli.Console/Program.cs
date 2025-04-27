// See https://aka.ms/new-console-template for more information

using Cocona;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Sample;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddTransient<ICommand<SampleCommandParams, CommandResult>, SampleCommand>();

builder.Services.Configure<CoconaAppOptions>(options =>
{
    options.TreatPublicMethodsAsCommands = false;
});

var app = builder.Build();
app.AddCommands<SampleCommand>();
await app.RunAsync();
