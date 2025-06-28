using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using Cocona;
using DevopsCli.Core.Tools.Dotnet;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.Dotnet.Restore;

internal sealed class RestoreCommand : ICommand<RestoreCommandParams, Result>
{
    private readonly ILogger<RestoreCommand> _logger;
    private readonly IDotnetTool _dotnetTool;
    
    public RestoreCommand(ILogger<RestoreCommand> logger, IDotnetTool dotnetTool)
    {
        _logger = logger;
        _dotnetTool = dotnetTool;
    }

    [Command("Restore")]
    public async Task<int> RunCommandAsync(RestoreCommandParams commandParams, [FromService] CoconaAppContext context)
    {
        var result = await RunAsync(commandParams, context.CancellationToken);

        result.LogErrorsIfAny(_logger);
        
        return result.ToCliExitCode();
    }
    
    public async Task<Result> RunAsync(RestoreCommandParams commandParams, CancellationToken cancellationToken)
    {
        var validationResult = commandParams.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await _dotnetTool.RestoreAsync(commandParams.SolutionPath, cancellationToken);

        return result;
    }
}
