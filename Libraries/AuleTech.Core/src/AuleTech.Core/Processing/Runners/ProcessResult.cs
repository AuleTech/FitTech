namespace AuleTech.Core.Processing.Runners;

public class ProcessResult
{
    public ProcessResult(int exitCode
        , string output)
    {
        ExitCode = exitCode;
        Output = output;
    }

    public int ExitCode { get; }
    public string Output { get; }
    public bool Completed => ExitCode == 0;

    public bool Errored()
    {
        return ExitCode != 0;
    }

    public void ThrowIfErrored()
    {
        if (Errored())
        {
            throw new ApplicationException($"Execution failed with exit code {ExitCode}.");
        }
    }
}
