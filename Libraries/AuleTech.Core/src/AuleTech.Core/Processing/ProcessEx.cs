using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace AuleTech.Core.Processing
{
    public static partial class ProcessEx
    {
        public static void KillGracefully(this Process? process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            var pid = process.Id;
            var processWasClosed = false;
            try
            {
                Console.WriteLine(() => $"Closing Process:({pid})");
                process.Close();
                process = Process.GetProcessById(pid);
                Console.WriteLine(() => $"Process:({pid}): Closed successfully");
            }
            catch (ArgumentException)
            {
                Console.WriteLine(() => $"Process {pid}: was already closed");
                processWasClosed = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(() => $"Process:({pid}): Close Failed: {ex.Message}");
            }

            if (!processWasClosed)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    Process? processById = null;
                    try
                    {
                        processById = Process.GetProcessById(pid);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine(() => $"Process:({pid}): was already closed");
                    }

                    Console.WriteLine(() => $"Killing Process:({process.Id})");
                    processById?.Kill();
                    Console.WriteLine(() => $"Process:({pid}): Killed successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(() => $"Process:({pid}): Kill Failed: {ex.Message}");
                }
            }
        }

        public static void KillGracefully(int? pid)
        {
            ArgumentNullException.ThrowIfNull(pid);
            
            if (TryGetProcessById(pid.Value, out var process))
            {
                KillGracefully(process!);
            }
        }

        public static int? GetProcessIdByWindowTitle(string title)
        {
            var processes = Process.GetProcesses()
                .OrderBy(_ => _.Id)
                .ToArray();
            return processes.SingleOrDefault(x => x.MainWindowTitle.Equals(title))
                ?.Id;
        }

        public static Process GetProcessByWindowHandler(int hWnd)
        {
            GetWindowThreadProcessId(hWnd, out var pId);
            return Process.GetProcessById(pId);
        }

        public static IEnumerable<Process> GetChildren(this Process parent
            , string? processNameFilter = null)
        {
            var runningProcesses = Process.GetProcesses()
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(processNameFilter))
            {
                runningProcesses = runningProcesses.Where(x => x.ProcessName.Contains(processNameFilter));
            }

            return runningProcesses.Where(x => x.GetParentId() == parent.Id)
                .ToArray();
        }

        public static int? GetParentId(this Process process)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    return ResolveWindows();
                }

                if (OperatingSystem.IsLinux())
                {
                    return ResolveLinux();
                }

                throw new PlatformNotSupportedException();
            }
            catch (Exception ex)
            {
                Console.WriteLine(() =>
                    $"Error when obtaining Parent from process {process.Id} '{ex.Message}' ... returning idle process");
                return 0;
            }
            
            int? ResolveWindows()
            {
                using var query =
                    new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ProcessId={process.Id}");

                var result = query
                    .Get()
                    .OfType<ManagementObject>()
                    .Select(p => Process.GetProcessById((int)(uint)p["ParentProcessId"]))
                    .FirstOrDefault();

                if (result is default(Process))
                {
                    return 0;
                }

                return result.Id;
            }

            int? ResolveLinux()
            {
                string? line;
                using (var reader = new StreamReader("/proc/" + process.Id + "/stat"))
                    line = reader.ReadLine();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    var endOfName = line.LastIndexOf(')');
                    var parts = line.Substring(endOfName)
                        .Split(new[] { ' ' }, 4);

                    if (parts.Length >= 3)
                    {
                        var ppid = int.Parse(parts[2]);
                        return ppid;
                    }
                }

                return null;
            }
        }

        public static bool TryGetProcessById(int pid
            , out Process? process)
        {
            process = null;
            try
            {
                process = Process.GetProcessById(pid);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(int hWnd
            , out int lpdwProcessId);
    }
}
