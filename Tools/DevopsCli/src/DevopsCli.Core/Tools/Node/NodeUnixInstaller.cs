using AuleTech.Core.Patterns;
using AuleTech.Core.Processing.Runners;

namespace DevopsCli.Core.Tools.Node;

public class NodeUnixInstaller : IInstaller<NodeTool>
{
    private const string VercelInstallUrl = "https://fnm.vercel.app/install";

    private readonly IProcessRunner _processRunner;

    public NodeUnixInstaller(IProcessRunner processRunner)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new NotSupportedException();
        }

        _processRunner = processRunner;
    }

    public bool IsSupported(PlatformID platform)
    {
        return platform is PlatformID.MacOSX or PlatformID.Unix;
    }

    public async Task<Result> InstallAsync(CancellationToken cancellationToken)
    {
        if (await IsInstalledAsync())
        {
            return Result.Success;
        }

        var result = await _processRunner.RunSequenceAsync([
            new KeyValuePair<string, string>("curl", $"-o- {VercelInstallUrl} | bash"),
            new KeyValuePair<string, string>("fnm", $"install {NodeTool.NodeVersion}"),
            new KeyValuePair<string, string>("echo",
                $"'eval \\\"$(fnm env --use-on-cd --shell {GetShell()})\\\"' >> ~/.{GetShell()}rc") //TODO: Update the file from code.
        ], cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        return await IsInstalledAsync() ? Result.Success : Result.Failure("Node installation failed check output");

        async Task<bool> IsInstalledAsync()
        {
            var nodeProcess = new AuleTechProcessStartInfo("node", "-v");

            var processResult = await _processRunner.RunBashAsync(nodeProcess, cancellationToken);

            return !processResult.Errored();
        }

        string GetShell()
        {
            return OperatingSystem.IsMacOS() ? "zsh" : "bashrc";
        }
    }
}
