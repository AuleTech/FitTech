using AuleTech.Core.Patterns;

namespace DevopsCli.Core.Tools.Node;

internal interface INodeTool : ITool
{
    Task<Result> NpmInstallAsync(string packageName, CancellationToken cancellationToken, bool isGlobal = true);
}
