namespace AuleTech.Core.Processing.Runners;

public interface IProcessRunner
{
    IEnumerable<string> GetCurrentProcessOutputLines();

    Task<ProcessResult> RunBashAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    ProcessResult RunBash(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    Task<ProcessResult> RunAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    ProcessResult Run(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    ProcessResult Run(string process
        , string arguments
        , bool appendOutputPrefix = true
        , CancellationToken cancellationToken = default);
}
