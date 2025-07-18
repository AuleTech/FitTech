﻿using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;

namespace DevopsCli.Core.Tools;

internal interface IInstaller<TTool> where TTool : ITool
{
    bool IsSupported(PlatformID platform);
    Task<Result> InstallAsync(CancellationToken cancellationToken);
}

internal interface IInstaller<TTool, in TConfig> where TConfig : class
    where TTool : ITool
{
    bool IsSupported(PlatformID platform);
    Task<Result> InstallAsync(TConfig config, CancellationToken cancellationToken);
}
