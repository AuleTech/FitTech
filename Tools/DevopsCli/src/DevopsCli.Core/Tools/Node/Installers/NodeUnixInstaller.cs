using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using AuleTech.Core.Processing.Runners;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Tools.Node.Installers;

internal sealed class NodeUnixInstaller : IInstaller<NodeTool>
{
    private const string VercelInstallUrl = "https://fnm.vercel.app/install";

    private readonly IProcessRunner _processRunner;
    private readonly ILogger<NodeTool> _logger;

    public NodeUnixInstaller(IProcessRunner processRunner, ILogger<NodeTool> logger)
    {
        _processRunner = processRunner;
        _logger = logger;
    }

    public bool IsSupported(PlatformID platform)
    {
        return platform is PlatformID.MacOSX or PlatformID.Unix;
    }

    public async Task<Result> InstallAsync(CancellationToken cancellationToken)
    {
        if (await IsInstalledAsync())
        {
            _logger.LogInformation("Node is already installed, skipping...");
            return Result.Success;
        }

        _logger.LogDebug("Downloading Node installer for version {Version}", NodeTool.NodeVersion);
        
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
