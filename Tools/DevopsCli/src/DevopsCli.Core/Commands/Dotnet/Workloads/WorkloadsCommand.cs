using AuleTech.Core.Patterns;
using Cocona;
using DevopsCli.Core.Tools.Dotnet;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.Dotnet.Workloads;

internal sealed class WorkloadsCommand : ICommand<WorkloadsCommandParams, Result>
{
    private readonly ILogger<WorkloadsCommand> _logger;
    private readonly IDotnetTool _dotnetTool;
    
    public WorkloadsCommand(ILogger<WorkloadsCommand> logger, IDotnetTool dotnetTool)
    {
        _logger = logger;
        _dotnetTool = dotnetTool;
    }

    [Command("RestoreWorkloads")]
    public async Task<int> RunCommandAsync(WorkloadsCommandParams commandParams, [FromService] CoconaAppContext context)
    {
        var result = await RunAsync(commandParams, context.CancellationToken);

        result.LogErrorsIfAny(_logger);
        
        return result.ToCliExitCode();
    }
    
    public async Task<Result> RunAsync(WorkloadsCommandParams commandParams, CancellationToken cancellationToken)
    {
        var result = await _dotnetTool.RestoreWorkloadsAsync(commandParams.Project, commandParams.RunAsAdministrator ,cancellationToken);

        return result;
    }
}
