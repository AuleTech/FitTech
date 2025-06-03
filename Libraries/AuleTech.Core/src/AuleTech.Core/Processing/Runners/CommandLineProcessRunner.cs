using System.Collections.Concurrent;
using System.Diagnostics;
using AuleTech.Core.Resiliency;
using Microsoft.Extensions.Logging;

namespace AuleTech.Core.Processing.Runners;

internal class CommandLineProcessRunner : IProcessRunner
{
    private readonly object _appendSyncLock = new();
    private readonly SemaphoreSlim _criticalSectionAsyncLock = new(1, 1);
    private readonly ILogger<CommandLineProcessRunner> _logger;

    private readonly ConcurrentQueue<string> _outputLines = new();
    private bool _lastMessageArrived;
    private Process? _process;

    public CommandLineProcessRunner() : this(new Logger<CommandLineProcessRunner>(new LoggerFactory()))
    {
    }

    public CommandLineProcessRunner(ILogger<CommandLineProcessRunner> logger)
    {
        _logger = logger;
    }

    public async Task<ProcessResult> RunBashAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true)
    {
        if (OperatingSystem.IsWindows())
        {
            return await RunGitBashAsync(startInfo, cancellationToken);
        }
        
        return await ExecuteAsync(
            new AuleTechProcessStartInfo("/bin/bash"
                , $"-l -c \"{(startInfo.RunAsAdministrator ? "sudo " : string.Empty)}{startInfo.FilePath} {startInfo.Arguments}\""
                , startInfo.WorkingDirectory
                , startInfo.Timeout
                , startInfo.StandardInput
                , startInfo.AddOutputToResult)
            , cancellationToken
            , appendOutputPrefix);
    }

    public async Task<ProcessResult> RunGitBashAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new InvalidOperationException("Only supported for Windows");
        }
        
        return await ExecuteAsync(
            new AuleTechProcessStartInfo($@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\Git\bin\bash.exe"
                , $"-l -c \"{startInfo.FilePath} {startInfo.Arguments}\""
                , startInfo.WorkingDirectory
                , startInfo.Timeout
                , startInfo.StandardInput
                , startInfo.AddOutputToResult)
            , cancellationToken
            , appendOutputPrefix);
    }

    public async Task<ProcessResult> RunAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken = default
        , bool appendOutputPrefix = true)
    {
        if (!OperatingSystem.IsWindows())
        {
            return await RunBashAsync(startInfo, cancellationToken);
        }
        
        return await ExecuteAsync(startInfo, cancellationToken, appendOutputPrefix);
    }


    private async Task<ProcessResult> ExecuteAsync(AuleTechProcessStartInfo startInfo
        , CancellationToken cancellationToken
        , bool appendOutputPrefix)
    {
        var scope =
            $"PROCESS {startInfo.FilePath} with ARGUMENTS {startInfo.Arguments} on WORKING FOLDER '{startInfo.WorkingDirectory ?? Environment.CurrentDirectory}'";
        ProcessResult result;

        if (!startInfo.UseShellExecute)
        {
            await _criticalSectionAsyncLock.WaitAsync(cancellationToken);
            try
            {
                _process = StartProcess(startInfo);
                _logger.LogDebug($"pId({_process.Id}) - {scope}");

                result = await ExecuteEmbedded(startInfo.Timeout
                    , startInfo.StandardInput
                    , startInfo.AddOutputToResult
                    , _process
                    , appendOutputPrefix
                    , cancellationToken);

                _process.Dispose();
                _process = null;
                _lastMessageArrived = false;
            }
            finally
            {
                _criticalSectionAsyncLock.Release();
            }
        }
        else
        {
            result = new ProcessResult(0
                , $"{nameof(startInfo.UseShellExecute)}={startInfo.UseShellExecute} hence the process has been opened on an interactive console");
        }


        return result;
    }

    private async Task<ProcessResult> ExecuteEmbedded(TimeSpan? timeout
        , string[]? standardInput
        , bool addOutputToResult
        , Process process
        , bool appendOutputPrefix
        , CancellationToken cancellationToken
    )
    {
        ProcessResult result;
        try
        {
            foreach (var stdInput in standardInput ?? Array.Empty<string>())
            {
                await process.StandardInput.WriteLineAsync(stdInput);
            }

            // if the process is cmd.exe and using standard input, exit should be the last input
            if (standardInput?.Length > 0)
            {
                await process.StandardInput.WriteLineAsync("exit");
            }

            if (cancellationToken != default)
            {
                cancellationToken.Register(() => Kill(process));
            }

            using (var combinedStream = new StringWriter())
            {
                using (var runProcess = Task.Factory.StartNew(() =>
                           {
                               bool runResult;
                               if (timeout != null)
                               {
                                   runResult = process.WaitForExit((int)timeout.Value.TotalMilliseconds);
                               }
                               else
                               {
                                   process.WaitForExit();
                                   runResult = true;
                               }

                               return runResult;
                           }
                           , cancellationToken))
                using (var outputReader =
                       process.StartInfo.UseShellExecute && (addOutputToResult || Debugger.IsAttached)
                           ? Task.Factory.StartNew(AppendLines!
                               , Tuple.Create(combinedStream, "stdout", process.StandardOutput, appendOutputPrefix)
                               , cancellationToken)
                           : Task.CompletedTask)
                using (var errorReader = process.StartInfo.UseShellExecute && addOutputToResult
                           ? Task.Factory.StartNew(AppendLines!
                               , Tuple.Create(combinedStream, "stderr", process.StandardError, appendOutputPrefix)
                               , cancellationToken)
                           : Task.CompletedTask)
                {
                    var waitResult = await runProcess;

                    if (!waitResult)
                    {
                        _logger.LogDebug($"pId({process.Id}) - will be killed as no wait was set.");
                        ProcessEx.KillGracefully(process.Id);
                    }

                    await Task.WhenAll(outputReader, errorReader);

                    if (!waitResult)
                    {
                        throw new TimeoutException(
                            $"Process wait timeout expired. Timeout = {timeout}{Environment.NewLine}{combinedStream}.");
                    }

                    await WaitForLastOutputToArriveAsync();

                    result = new ProcessResult(process.ExitCode, process.StartInfo.UseShellExecute
                        ? combinedStream.ToString()
                        : string.Join(Environment.NewLine, _outputLines.ToArray()));
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation($"pId({process?.Id}) - The operation was cancelled.");

            Kill(process);
            throw;
        }

        return result;

        async Task WaitForLastOutputToArriveAsync()
        {
            var attemps = 5;

            while (!_lastMessageArrived && attemps > 0)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                attemps--;
            }
        }
    }

    private static void Kill(Process? process)
    {
        if (process == null)
        {
            throw new ArgumentNullException(nameof(process));
        }

        // ReSharper disable once MethodSupportsCancellation
        Task.Run(() => ResilientOperations.Default.RetryIfNeeded(_ =>
            {
                try
                {
                    ProcessEx.KillGracefully(process.Id);
                }
                catch (InvalidOperationException e)
                {
                    if (!e.Message.Contains("No process is associated with this object"))
                    {
                        throw;
                    }
                }
            }
            , TimeSpan.FromSeconds(30)
            , TimeSpan.Zero
            , e => Console.WriteLine($"{e}")
        ));
    }

    private Process StartProcess(AuleTechProcessStartInfo startInfo)
    {
        var process = new Process();


        var info = process.StartInfo;
        info.UseShellExecute = startInfo.UseShellExecute;
        info.CreateNoWindow = true;
        info.RedirectStandardError = !startInfo.UseShellExecute;
        info.RedirectStandardOutput = !startInfo.UseShellExecute;
        info.FileName = startInfo.FilePath;
        info.Arguments = startInfo.Arguments;
        info.RedirectStandardInput = !startInfo.UseShellExecute; //&& startInfo.StandardInput?.Length > 0;

        if (!info.UseShellExecute)
        {
            process.EnableRaisingEvents = info.RedirectStandardError
                                          && info.RedirectStandardInput
                                          && info.RedirectStandardOutput;

            process.OutputDataReceived += OutputDataReceived;
            process.ErrorDataReceived += OutputDataReceived;
        }

        if (startInfo.WorkingDirectory != null)
        {
            info.WorkingDirectory = startInfo.WorkingDirectory;
        }

        if (startInfo.RunAsAdministrator)
        {
            info.Verb = "runAs";
        }

        _logger.LogInformation($"{process.StartInfo.FileName} {startInfo.Arguments}");
        process.Start();

        if (!process.HasExited)
        {
            if (process.EnableRaisingEvents)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            process.PriorityClass = startInfo.Priority;
        }

        return process;
    }

    private void OutputDataReceived(object sender
        , DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            _logger.LogInformation(e.Data);
        }

        _outputLines.Enqueue(e.Data ?? string.Empty);

        CheckIfLastOutputMessage();

        void CheckIfLastOutputMessage()
        {
            if (!_outputLines.IsEmpty && e.Data is null)
            {
                _lastMessageArrived = true;
            }
        }
    }

    private void AppendLines(object packedParams)
    {
        var paramsTuple = (Tuple<StringWriter, string, StreamReader, bool>)packedParams;
        var writer = paramsTuple.Item1;
        var marker = paramsTuple.Item2;
        var reader = paramsTuple.Item3;
        var appendOutputPrefix = paramsTuple.Item4;
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lock (_appendSyncLock)
            {
                if (appendOutputPrefix)
                {
                    writer.WriteLine($"{marker} {DateTime.Now}: {line}");
                }
                else
                {
                    writer.WriteLine(line);
                }

                var line1 = line;
                _logger.LogInformation(line1);
            }
        }
    }
}
