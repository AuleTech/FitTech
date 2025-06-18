using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using AuleTech.Core.Processing.Runners;
using AuleTech.Core.Processing.Runners.Factory;
using AuleTech.Core.System.IO.FileSystem;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Tools.Node.Installers;

internal sealed class NodeWindowsInstaller : IInstaller<NodeTool>
{
    private const string NodeMsiDownloadUrl =
        $"https://nodejs.org/dist/{NodeTool.NodeVersion}/node-{NodeTool.NodeVersion}-x64.msi";

    private readonly ILogger<NodeTool> _logger;
    private readonly IProcessRunnerFactory _processRunnerFactory;
    private readonly ISystemIo _systemIo;
    public string NodeInstallationFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs");
    public NodeWindowsInstaller(IProcessRunnerFactory processRunnerFactory, ISystemIo systemIo, ILogger<NodeTool> logger)
    {
        _processRunnerFactory = processRunnerFactory;
        _systemIo = systemIo;
        _logger = logger;
    }

    public bool IsSupported(PlatformID platform)
    {
        return platform == PlatformID.Win32NT;
    }

    public async Task<Result> InstallAsync(CancellationToken cancellationToken)
    {
        if (await IsInstalledAsync())
        {
            _logger.LogInformation("Node is already installed, skipping...");
            return Result.Success;
        }

        _logger.LogDebug("Downloading Node installer for version {Version}", NodeTool.NodeVersion);
        var tempFolder = _systemIo.Path.GetTempPath(Guid.CreateVersion7()
            .ToString()[..^3], true, true);
        var msiPath = _systemIo.Path.Combine(tempFolder, "WindowsNode.msi");
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(NodeMsiDownloadUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        _logger.LogDebug("Download succeeded, starting installation...");
        await _systemIo.File.WriteStreamAsync(msiPath, await response.Content.ReadAsStreamAsync(cancellationToken)
            , FileMode.CreateNew, cancellationToken);

        var processInfo = new AuleTechProcessStartInfo("msiexec.exe", $"/i {msiPath} /passive /qn"
            , runAsAdministrator: true);
        var result = await _processRunnerFactory.GetOne().RunAsync(processInfo, cancellationToken);

        if (result.Errored())
        {
            return Result.Failure(result.Output);
        }

        return await IsInstalledAsync() ? Result.Success : Result.Failure("Node couldn't be installed");

        async Task<bool> IsInstalledAsync()
        {
            try
            {
                var nodeProcess =
                    new AuleTechProcessStartInfo(Path.Combine(NodeInstallationFolder, "node"), "--version");

                var processResult = await _processRunnerFactory.GetOne().RunAsync(nodeProcess, cancellationToken);

                return !processResult.Errored();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
