using AuleTech.Core.Patterns;
using AuleTech.Core.Processing.Runners;
using DevopsCli.Core.Tools.Node;

namespace DevopsCli.Core.Tools.Nswag;

internal sealed class NSwagMultiplatformInstaller : IInstaller<NSwagTool>
{
    private readonly INodeTool _nodeTool;
    private readonly IProcessRunner _processRunner;

    public NSwagMultiplatformInstaller(INodeTool nodeTool, IProcessRunner processRunner)
    {
        _nodeTool = nodeTool;
        _processRunner = processRunner;
    }

    public bool IsSupported(PlatformID platform)
    {
        return true;
    }

    public async Task<Result> InstallAsync(CancellationToken cancellationToken)
    {
        var isInstalled = await IsInstalledAsync();

        if (isInstalled.Succeeded)
        {
            return isInstalled;
        }

        var result = await _nodeTool.NpmInstallAsync("nswag", cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        return await IsInstalledAsync();

        async Task<Result> IsInstalledAsync()
        {
            var process = new AuleTechProcessStartInfo("nswag", "version");
            var processResult = await _processRunner.RunAsync(process, cancellationToken);
            
            return processResult.ToResult();
        }
    }
}
