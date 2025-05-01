using System.Diagnostics;

namespace AuleTech.Core.Processing.Runners;

public class PlatformProcessStartInfo
{
    public PlatformProcessStartInfo(string filePath
        , string arguments
        , string? workingDirectory = null
        , TimeSpan? timeout = null
        , string[]? standardInput = default
        , bool addOutputToResult = false
        , bool useShellExecute = false
        , bool runAsAdministrator = false
        , ProcessPriorityClass priority = ProcessPriorityClass.Normal)
    {
        FilePath = filePath;
        Arguments = arguments;
        WorkingDirectory = workingDirectory;
        Timeout = timeout;
        StandardInput = standardInput;
        AddOutputToResult = addOutputToResult;
        UseShellExecute = useShellExecute;
        RunAsAdministrator = runAsAdministrator;
        Priority = priority;
    }

    public string FilePath { get; }
    public string Arguments { get; }
    public string? WorkingDirectory { get; }
    public TimeSpan? Timeout { get; }
    public string[]? StandardInput { get; }
    public bool AddOutputToResult { get; }
    public bool UseShellExecute { get; }
    public bool RunAsAdministrator { get; }
    public ProcessPriorityClass Priority { get; }
}
