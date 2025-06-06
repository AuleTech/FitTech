using AuleTech.Core.Patterns;
using AuleTech.Core.Processing.Runners;

namespace DevopsCli.Core.Tools.Node;

internal sealed class NodeTool : INodeTool
{
    public const string NodeVersion = "v22.15.0";
    private readonly IInstaller<NodeTool> _installer;
    private readonly IProcessRunner _processRunner;
    private string NodeInstallationFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs");
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

        var process = new AuleTechProcessStartInfo(GetNpmCommand(), $"install{(isGlobal ? " -g " : " ")}{packageName}", workingDirectory: workingDir, runAsAdministrator: OperatingSystem.IsWindows()); //TODO: Just for now
        var result = await _processRunner.RunAsync(process, cancellationToken);

        return result.ToResult();
    }

    private string GetNpmCommand() =>
        OperatingSystem.IsWindows() ? Path.Combine(NodeInstallationFolder, "npm.cmd") : "npm";
}
