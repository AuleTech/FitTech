using AuleTech.Core.Patterns;

namespace DevopsCli.Core.Tools.Dotnet;

internal interface IDotnetTool : ITool
{
    Task<Result> RestoreAsync(string solutionPath, CancellationToken cancellationToken);
    Task<Result> BuildAsync(string solutionPath, CancellationToken cancellationToken);
    Task<Result> TestAsync(string projectPath, CancellationToken cancellationToken);
    Task<Result> RestoreWorkloadsAsync(string projectPath, bool runAsAdmin, CancellationToken cancellationToken);
}
