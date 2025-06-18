using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using AuleTech.Core.Processing.Runners;
using AuleTech.Core.System.IO.FileSystem;

namespace DevopsCli.Core.Tools.Dotnet;

internal sealed class DotnetTool : IDotnetTool
{
    private readonly IProcessRunner _processRunner;
    private readonly ISystemIo _systemIo;
    
    public DotnetTool(IProcessRunner processRunner, ISystemIo systemIo)
    {
        _processRunner = processRunner;
        _systemIo = systemIo;
    }

    public async Task<Result> RestoreAsync(string solutionPath, CancellationToken cancellationToken)
    {
        if (!_systemIo.File.Exists(solutionPath))
        {
            return Result.Failure($"File {solutionPath} does not exist");
        }
        
        var processInfo = new AuleTechProcessStartInfo("dotnet", $"restore {solutionPath}");

        var result = await _processRunner.RunAsync(processInfo, cancellationToken);

        return result.ToResult();
    }
    
    public async Task<Result> BuildAsync(string solutionPath, CancellationToken cancellationToken)
    {
        if (!_systemIo.File.Exists(solutionPath))
        {
            return Result.Failure($"File {solutionPath} does not exist");
        }
        
        var processInfo = new AuleTechProcessStartInfo("dotnet", $"build {solutionPath} --no-restore");

        var result = await _processRunner.RunAsync(processInfo, cancellationToken);

        return result.ToResult();
    }

    public async Task<Result> TestAsync(string projectPath, CancellationToken cancellationToken)
    {
        if (!_systemIo.File.Exists(projectPath))
        {
            return Result.Failure($"Project {projectPath} does not exist");
        }
        
        var processInfo = new AuleTechProcessStartInfo("dotnet", $"test {projectPath} --no-restore --no-build");

        var result = await _processRunner.RunAsync(processInfo, cancellationToken);

        return result.ToResult();
    }

    public async Task<Result> RestoreWorkloadsAsync(string projectPath, bool runAsAdmin, CancellationToken cancellationToken)
    {
        var processInfo = new AuleTechProcessStartInfo("dotnet", $"workload restore {projectPath}", runAsAdministrator: runAsAdmin);
        
        var result = await _processRunner.RunAsync(processInfo, cancellationToken);

        return result.ToResult();
    }
}
