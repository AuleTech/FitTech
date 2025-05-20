using AuleTech.Core.Patterns;
using Cocona;
using DevopsCli.Core.Tools.Dotnet;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.Dotnet.Build;

internal sealed class BuildCommand : ICommand<BuildCommandParams, Result>
{
    private readonly IDotnetTool _dotnetTool;
    private readonly ILogger<BuildCommandParams> _logger;
    
    public BuildCommand(IDotnetTool dotnetTool, ILogger<BuildCommandParams> logger)
    {
        _dotnetTool = dotnetTool;
        _logger = logger;
    }

    [Command("Build")]
    public async Task<int> RunCommandAsync(BuildCommandParams commandParams, [FromService] CoconaAppContext context)
    {
        var result = await RunAsync(commandParams, context.CancellationToken);

        result.LogErrorsIfAny(_logger);
        
        return result.ToCliExitCode();
    }
    
    public async Task<Result> RunAsync(BuildCommandParams commandParams, CancellationToken cancellationToken)
    {
        var validationResult = commandParams.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await _dotnetTool.BuildAsync(commandParams.SolutionPath, cancellationToken);

        return result;
    }
}
