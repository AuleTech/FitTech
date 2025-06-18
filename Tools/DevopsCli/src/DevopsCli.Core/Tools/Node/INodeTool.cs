using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;

namespace DevopsCli.Core.Tools.Node;

public interface INodeTool : ITool
{
    Task<Result> NpmInstallAsync(string packageName, CancellationToken cancellationToken, bool isGlobal = true, string? workingDir = null);
}
