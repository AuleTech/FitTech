using AuleTech.Core.Patterns.Result;

namespace AuleTech.Core.Processing.Runners;

public static class IProcessRunnerExtensions
{
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
            var result = await runner.RunAsync(process,
                cancellationToken);

            if (result.Errored())
            {
                return Result.Failure(result.Output);
            }
        }

        return Result.Success;
    }
}
