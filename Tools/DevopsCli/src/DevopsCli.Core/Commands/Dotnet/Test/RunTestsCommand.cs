using AuleTech.Core.Patterns;
using Cocona;
using DevopsCli.Core.Tools.Dotnet;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.Dotnet.Tests;

internal sealed class RunTestsCommand : ICommand<RunTestsCommandParams, Result>
{
    private readonly IDotnetTool _dotnetTool;
    private readonly ILogger<RunTestsCommand> _logger;

    public RunTestsCommand(IDotnetTool dotnetTool, ILogger<RunTestsCommand> logger)
    {
        _dotnetTool = dotnetTool;
        _logger = logger;
    }

    [Command("test")]
    public async Task<int> RunCommandAsync(RunTestsCommandParams commandParams, [FromService] CoconaAppContext context)
    {
        var result = await RunAsync(commandParams, context.CancellationToken);

        result.LogErrorsIfAny(_logger);
        
        return result.ToCliExitCode();
    }
    
    public async Task<Result> RunAsync(RunTestsCommandParams commandParams, CancellationToken cancellationToken)
    {
        var validationResult = commandParams.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        return await _dotnetTool.TestAsync(commandParams.ProjectPath, cancellationToken);
    }
}
