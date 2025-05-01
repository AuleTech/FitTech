namespace AuleTech.Core.Processing.Runners;

public interface IProcessRunner
{
    IEnumerable<string> GetCurrentProcessOutputLines();

    Task<ProcessResult> RunBashAsync(PlatformProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    ProcessResult RunBash(PlatformProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    Task<ProcessResult> RunAsync(PlatformProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    ProcessResult Run(PlatformProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    ProcessResult Run(string process
        , string arguments
        , bool appendOutputPrefix = true
        , CancellationToken cancellationToken = default);
}
