namespace AuleTech.Core.Processing.Runners;

public interface IProcessRunner
{
    Task<ProcessResult> RunBashAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    Task<ProcessResult> RunAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);

    Task<ProcessResult> RunGitBashAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true);
    
}
