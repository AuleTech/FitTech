namespace AuleTech.Core.Processing.Runners;

public static class IProcessRunnerExtensions
{
	public static Task RunGitBashAsync(this IProcessRunner target, string arguments
	                                         , CancellationToken cancellationToken
	                                         , string? workingFolder = null) =>
		RunGitBashAndGetResponseAsync(target, arguments, cancellationToken, workingFolder);

	public static async Task<string> RunGitBashAndGetResponseAsync(this IProcessRunner target, string arguments
	                                                               , CancellationToken cancellationToken
	                                                               , string? workingFolder = null)
	{
		var gitBash = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\Git\bin\bash.exe";
		var result = await target.RunAsync(
			new PlatformProcessStartInfo(
				gitBash
				, $"-l -c \"{arguments}\""
				, addOutputToResult: true
				, runAsAdministrator: true
				, workingDirectory: workingFolder
			), cancellationToken);

		if (result.ExitCode != 0)
		{
			throw new ApplicationException($"Git Bash {arguments}. Failed with exit code {result.ExitCode}.{Environment.NewLine}{result.Output}");
		}

		return result.Output;
	}
}
