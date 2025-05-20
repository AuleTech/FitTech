using AuleTech.Core.Patterns;

namespace DevopsCli.Core.Tools.Node;

public interface INodeTool : ITool
{
    Task<Result> NpmInstallAsync(string packageName, CancellationToken cancellationToken, bool isGlobal = true, string? workingDir = null);
}
