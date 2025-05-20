using AuleTech.Core.Patterns;
using AuleTech.Core.Processing.Runners;

namespace DevopsCli.Core.Tools.Node;

internal sealed class NodeTool : INodeTool
{
    public const string NodeVersion = "v22.15.0";
    private readonly IInstaller<NodeTool> _installer;
    private readonly IProcessRunner _processRunner;
    public NodeTool(IInstaller<NodeTool> installer, IProcessRunner processRunner)
    {
        _installer = installer;
        _processRunner = processRunner;
    }

    public async Task<Result> NpmInstallAsync(string packageName, CancellationToken cancellationToken, bool isGlobal = true, string? workingDir = null)
    {
        var installResult = await _installer.InstallAsync(cancellationToken);

        if (!installResult.Succeeded)
        {
            return installResult;
        }

        var process = new AuleTechProcessStartInfo("npm", $"install{(isGlobal ? " -g " : " ")}{packageName}", workingDirectory: workingDir);
        var result = await _processRunner.RunAsync(process, cancellationToken);

        return result.ToResult();
    }
}
