using System.Diagnostics;
using System.Runtime.InteropServices;
using AuleTech.Core.Processing;
using AuleTech.Core.Resiliency;
using Microsoft.Extensions.Logging;

namespace AuleTech.Core.System.IO.FileSystem.Files
{
	internal partial class SystemIoFileProxy
	{
		public async Task<IEnumerable<Process>> GetProcessesLockingFileAsync(string filePath
		                                                                     , CancellationToken cancellationToken)
		{
			if (!OperatingSystem.IsWindows())
			{
				throw new PlatformNotSupportedException();
			}


			return await Task.Run(() =>
				{
					var processes = Array.Empty<Process>();
					try
					{
						processes = LockedFileHandler.GetProcessesLocking(filePath)
							.ToArray();
					}
					catch (Exception ex)
					{
						Console.WriteLine(() =>
							$"Error getting processes locking files. {Environment.NewLine}Reason: {ex.Message}");
					}

					return processes;
				}
				, cancellationToken);
		}

		public async Task WaitForFileReleaseAsync(string filePath
		                                          , TimeSpan waitTime
		                                          , bool killProcessIfNotReleased)
		{
			if (IsLocked() && killProcessIfNotReleased)
			{
				if (!OperatingSystem.IsWindows())
				{
					throw new NotImplementedException(
						"The current platform unlock has not been implemented, can you do it?");
				}

				using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
				foreach (var process in await GetProcessesLockingFileAsync(filePath, cts.Token))
				{
					try
					{
						ResilientOperations.Default.RetryIfNeeded(p => { process.KillGracefully(); }
							, 3
						);
					}
					catch (Exception ex)
					{
						Console.WriteLine(() =>
							$"Could not kill the process({process.Id}) locking {filePath}.Reason:{Environment.NewLine} {ex}");
					}
				}
			}

			bool IsLocked()
			{
				var locked = false;
				try
				{
					using var _ = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Write);
				}
				catch (IOException)
				{
					using var released = new ManualResetEventSlim(false);
					using (var fileSystemWatcher =
						new FileSystemWatcher(Path.GetDirectoryName(filePath)!)
						{
							EnableRaisingEvents = true
						})
					{
						fileSystemWatcher.Changed +=
							(o
							 , e) =>
							{
								if (Path.GetFullPath(e.FullPath) == Path.GetFullPath(filePath))
								{
									released.Set();
								}
							};
						locked = !released.Wait(waitTime);
					}
				}

				return locked;
			}
		}

		/// <remarks>
		///     taken from https://stackoverflow.com/questions/1304/how-to-check-for-file-lock
		///     (no copyright in code at time of viewing)
		/// </remarks>
		private static class LockedFileHandler
		{
			private const int RmRebootReasonNone = 0;
			private const int CCH_RM_MAX_APP_NAME = 255;
			private const int CCH_RM_MAX_SVC_NAME = 63;

			[DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
			private static extern int RmRegisterResources(uint pSessionHandle
			                                              , uint nFiles
			                                              , string[] rgsFilenames
			                                              , uint nApplications
			                                              , [In] RM_UNIQUE_PROCESS[]? rgApplications
			                                              , uint nServices
			                                              , string[]? rgsServiceNames);

			[DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
			private static extern int RmStartSession(out uint pSessionHandle
			                                         , int dwSessionFlags
			                                         , string strSessionKey);

			[DllImport("rstrtmgr.dll")]
			private static extern int RmEndSession(uint pSessionHandle);

			[DllImport("rstrtmgr.dll")]
			private static extern int RmGetList(uint dwSessionHandle
			                                    , out uint pnProcInfoNeeded
			                                    , ref uint pnProcInfo
			                                    , [In] [Out] RM_PROCESS_INFO[]? rgAffectedApps
			                                    , ref uint lpdwRebootReasons);

			/// <summary>
			///     Find out what process(es) have a lock on the specified file.
			/// </summary>
			/// <param name="path">Path of the file.</param>
			/// <returns>Processes locking the file</returns>
			/// <remarks>
			///     See also:
			///     http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
			/// </remarks>
			public static IEnumerable<Process> GetProcessesLocking(string path)
			{
				uint handle;
				var key = Guid.NewGuid()
					.ToString();
				var processes = new List<Process>();

				var res = RmStartSession(out handle, 0, key);

				if (res != 0)
				{
					throw new Exception("Could not begin restart session. Unable to determine file locker.");
				}

				try
				{
					const int ERROR_MORE_DATA = 234;
					uint pnProcInfo = 0, lpdwRebootReasons = RmRebootReasonNone;

					var resources = new[]
					{
						path
					}; // Just checking on one resource.

					res = RmRegisterResources(handle, (uint) resources.Length, resources, 0, null, 0, null);

					if (res != 0)
					{
						throw new Exception("Could not register resource.");
					}

					//Note: there's a race condition here -- the first call to RmGetList() returns
					// the total number of process. However, when we call RmGetList() again to get
					// the actual processes this number may have increased.
					res = RmGetList(handle, out var pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

					if (res == ERROR_MORE_DATA)
					{
						var processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
						pnProcInfo = pnProcInfoNeeded;

						res = RmGetList(handle
							, out pnProcInfoNeeded
							, ref pnProcInfo
							, processInfo
							, ref lpdwRebootReasons);

						if (res == 0)
						{
							processes = new List<Process>((int) pnProcInfo);
							for (var i = 0; i < pnProcInfo; i++)
							{
								try
								{
									processes.Add(Process.GetProcessById(processInfo[i]
										.Process.dwProcessId));
								}
								catch (ArgumentException)
								{
									// catch the error -- in case the process is no longer running
								}
							}
						}
						else
						{
							throw new Exception("Could not list processes locking resource.");
						}
					}
					else if (res != 0)
					{
						throw new Exception("Could not list processes locking resource. Failed to get size of result.");
					}
				}
				finally
				{
					RmEndSession(handle);
				}

				return processes;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct RM_UNIQUE_PROCESS
			{
				public readonly int dwProcessId;
				public readonly FILETIME ProcessStartTime;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct FILETIME
			{
				public readonly int dwHighDateTime;
				public readonly int dwLowDateTime;
			}

			private enum RM_APP_TYPE
			{
				RmUnknownApp = 0
				, RmMainWindow = 1
				, RmOtherWindow = 2
				, RmService = 3
				, RmExplorer = 4
				, RmConsole = 5
				, RmCritical = 1000
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			private struct RM_PROCESS_INFO
			{
				public readonly RM_UNIQUE_PROCESS Process;

				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
				public readonly string strAppName;

				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
				public readonly string strServiceShortName;

				public readonly RM_APP_TYPE ApplicationType;
				public readonly uint AppStatus;
				public readonly uint TSSessionId;

				[MarshalAs(UnmanagedType.Bool)]
				public readonly bool bRestartable;
			}
		}
	}
}
