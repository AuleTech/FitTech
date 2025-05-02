using AuleTech.Core.Patterns;

namespace AuleTech.Core.Processing.Runners;

public static class IProcessRunnerExtensions
{
    public static Task RunGitBashAsync(this IProcessRunner target, string arguments
        , CancellationToken cancellationToken
        , string? workingFolder = null)
    {
        return RunGitBashAndGetResponseAsync(target, arguments, cancellationToken, workingFolder);
    }

    public static async Task<string> RunGitBashAndGetResponseAsync(this IProcessRunner target, string arguments
        , CancellationToken cancellationToken
        , string? workingFolder = null)
    {
        var gitBash = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\Git\bin\bash.exe";
        var result = await target.RunAsync(
            new AuleTechProcessStartInfo(
                gitBash
                , $"-l -c \"{arguments}\""
                , addOutputToResult: true
                , runAsAdministrator: true
                , workingDirectory: workingFolder
            ), cancellationToken);

        if (result.ExitCode != 0)
        {
            throw new ApplicationException(
                $"Git Bash {arguments}. Failed with exit code {result.ExitCode}.{Environment.NewLine}{result.Output}");
        }

        return result.Output;
    }

    public static async Task<Result> RunSequenceAsync(this IProcessRunner runner,
        IEnumerable<KeyValuePair<string, string>> commandArgumentsPairs, CancellationToken cancellationToken)
    {
        var keyValuePairs = commandArgumentsPairs as KeyValuePair<string, string>[] ?? commandArgumentsPairs.ToArray();
        
        if (!keyValuePairs.Any())
        {
            return Result.Success;
        }

        foreach (var commandArgumentsPair in keyValuePairs)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var process = new AuleTechProcessStartInfo(commandArgumentsPair.Key, commandArgumentsPair.Value);
            var result = await runner.RunBashAsync(process,
                cancellationToken);

            if (result.Errored())
            {
                return Result.Failure(result.Output);
            }
        }
        
        return Result.Success;
    } 
}
