using System.Runtime.InteropServices;
using AuleTech.Core.Processing;
using AuleTech.Core.Processing.Runners;

namespace AuleTech.Core.System.IO.FileSystem.Directories;

internal partial class SystemIoDirectoryProxy
{
    private class DirectoryUnlocker
	{
		public static readonly DirectoryUnlocker Instance = new();

		private DirectoryUnlocker() { }

		public async Task UnlockFromLocalProcessesAsync(string folder
		                                                , CancellationToken cancellationToken)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				await UnlockFromLocalProcessesWindowsAsync(folder, cancellationToken);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				await UnlockFromLocalProcessesLinuxAsync(folder, cancellationToken);
			}
			else
			{
				throw new PlatformNotSupportedException();
			}
		}

		private async Task UnlockFromLocalProcessesWindowsAsync(string folder
		                                                        , CancellationToken cancellationToken)
		{
			await DownloadHandleIfNotExist();

			var processHandles = (await GetProcessHandlesAsync(folder)).ToArray();
			foreach (var processHandle in processHandles)
			{
				await CloseHandleAsync(processHandle);
			}

			var processIds = processHandles.GroupBy(x => x.Pid)
				.Select(x => x.Key)
				.ToArray();
			foreach (var processId in processIds)
			{
				Console.WriteLine(() => $"Closing process with id {processId}");
				ProcessEx.KillGracefully(processId);
			}

			async Task DownloadHandleIfNotExist()
			{
				if (!SystemIoProxy.Default.File.Exists(ResolveHandleExePath()))
				{
					using var semaphore = new SemaphoreSlim(1);
					await semaphore.WaitAsync(cancellationToken);
					try
					{
						Console.WriteLine(() => "Unlocker: provisioning...");
						var handleUrl = new Uri("https://download.sysinternals.com/files/Handle.zip");
						using var httpClient = new HttpClient();
						using var response = await httpClient.GetAsync(handleUrl, cancellationToken);
						response.EnsureSuccessStatusCode();
						using var fileStream = await response.Content.ReadAsStreamAsync();
						SystemIoProxy.Default.ZipCompression.ExtractToDirectory(fileStream
							, ResolveUnlockerFolderPath());

						Console.WriteLine(() => "Unlocker: accepting eula...");
						var result = await RunHandleExe("-nobanner -accepteula");
						if (result.ExitCode != 0)
						{
							throw new Exception("Error while accepting eula Handle.exe");
						}
					}
					finally
					{
						semaphore.Release();
					}
				}
			}

			async Task<IEnumerable<ProcessHandle>> GetProcessHandlesAsync(string folder)
			{
				var searchText = SystemIoProxy.Default.Path.GetRelativePath("C:\\", folder);
				var arguments = $"\"{searchText}\" -v -nobanner";

				var result = await RunHandleExe(arguments);
				if (result.ExitCode == 0)
				{
					if (result.Output.Trim() != "No matching handles found.")
					{
						return GetProcessHandlesFromOutput(result.Output);
					}
				}

				return Array.Empty<ProcessHandle>();
			}

			IEnumerable<ProcessHandle> GetProcessHandlesFromOutput(string processOutput)
			{
				var collection = new List<ProcessHandle>();
				var lines = processOutput.Split(Environment.NewLine.ToCharArray()
					, StringSplitOptions.RemoveEmptyEntries);
				for (var i = 1; i < lines.Length; i++)
				{
					var values = lines[i]
						.Split(',');

					if (values.Length >= 5)
					{
						var handle = new ProcessHandle
						{
							Process = values[0]
							, Pid = int.Parse(values[1])
							, Type = values[2]
							, Handle = values[3]
							, Name = values[4]
						};
						collection.Add(handle);
					}
				}

				return collection;
			}

			async Task CloseHandleAsync(ProcessHandle handle)
			{
				Console.WriteLine(() =>
					$"Closing process handle with name \"{handle.Name}\" for process {handle.Pid}/{handle.Process}");
				var arguments = $"-p {handle.Pid} -c {handle.Handle} -y";
				await RunHandleExe(arguments);
			}

			async Task<ProcessResult> RunHandleExe(string arguments)
			{
				var processInfo = new PlatformProcessStartInfo(ResolveHandleExePath(), arguments);
				var processRunner = new CommandLineProcessRunner(); //TODO: Fix
				return await processRunner.RunAsync(processInfo, cancellationToken);
			}

			string ResolveUnlockerFolderPath() => SystemIoProxy.Default.Path.GetPlatformTempPath("unlocker",true);

			string ResolveHandleExePath() =>
				SystemIoProxy.Default.Path.Combine(ResolveUnlockerFolderPath(), "handle.exe");
		}

		private async Task UnlockFromLocalProcessesLinuxAsync(string? folder
		                                                      , CancellationToken cancellationToken)
		{
			await DownloadLsofIfNotExist();

			var processIds = await GetOpenFilePidAsync(folder);
			foreach (var processId in processIds)
			{
				Console.WriteLine($"Closing process with id {processId}");
				await RunBashAsync("kill", $"-9 {processId}");
			}

			async Task DownloadLsofIfNotExist()
			{
				if (!await IsLsofInstalledAsync())
				{
					var processRunner = new CommandLineProcessRunner();

					var processInfo = new PlatformProcessStartInfo("/bin/bash", "sudo apt install -y lsof");
					var result = await processRunner.RunBashAsync(processInfo, cancellationToken);
					if (result.ExitCode != 0)
					{
						throw new Exception("Error while installing lsof.");
					}

					processInfo = new PlatformProcessStartInfo("lsof", "-v");
					result = await processRunner.RunBashAsync(processInfo, cancellationToken);
					if (result.ExitCode != 0)
					{
						throw new Exception("Version check failed for lsof");
					}
				}
			}

			async Task<bool> IsLsofInstalledAsync() => (await RunBashAsync("which", "lsof")).ExitCode == 0;

			async Task<IEnumerable<int>> GetOpenFilePidAsync(string? searchFolder)
			{
				var result = await RunBashAsync("lsof", $"-t +D {searchFolder}");
				if (result.ExitCode == 0)
				{
					if (!string.IsNullOrWhiteSpace(result.Output))
					{
						return result.Output.Split(Environment.NewLine.ToCharArray()
								, StringSplitOptions.RemoveEmptyEntries)
							.Select(x => Convert.ToInt32(x));
					}
				}

				return Array.Empty<int>();
			}

			async Task<ProcessResult> RunBashAsync(string process
			                                       , string arguments)
			{
				var processInfo = new PlatformProcessStartInfo(process, arguments);
				var processRunner = new CommandLineProcessRunner();
				return await processRunner.RunBashAsync(processInfo, cancellationToken);
			}
		}

		private class ProcessHandle
		{
			public string Process { get; set; } = default!;
			public int Pid { get; set; }
			public string Handle { get; set; } = default!;
			public string Type { get; set; } = default!;
			public string Name { get; set; } = default!;
		}
	}
}
