using AuleTech.Core.Patterns;

namespace DevopsCli.Core.Tools;

public interface IInstaller<TTool> where TTool : ITool
{
    bool IsSupported(PlatformID platform);
    Task<Result> InstallAsync(CancellationToken cancellationToken);
}

public interface IInstaller<TTool, in TConfig> where TConfig : class
    where TTool : ITool
{
    bool IsSupported(PlatformID platform);
    Task<Result> InstallAsync(TConfig config, CancellationToken cancellationToken);
}
