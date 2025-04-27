using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace AuleTech.Core.Processing;

public static partial class ProcessEx
{
	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength, out int ReturnLength);

	public static void ThrowIfNotElevated()
	{
		if (!IsCurrentProcessRunningAsAdmin())
		{
			throw new InvalidOperationException(
				"This process requires to be running as an ADMIN to perform correctly its operation. Please start me with elevated permissions.");
		}
	}
	public static bool IsCurrentProcessRunningAsAdmin()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			throw new PlatformNotSupportedException("This method is only supported on Windows NT or later.");
		}

		using (WindowsIdentity.GetCurrent())
		{
			var tokenHandle = IntPtr.Zero;

			if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_QUERY, out tokenHandle))
			{
				throw new ApplicationException("Could not get process token.  Win32 Error Code: " + Marshal.GetLastWin32Error());
			}

			TOKEN_ELEVATION tokenElevation;
			tokenElevation.TokenIsElevated = 0;

			var tokenInfoLength = Marshal.SizeOf(typeof(TOKEN_ELEVATION));
			var tokenInfoPtr = Marshal.AllocHGlobal(tokenInfoLength);

			try
			{
				int returnLength;
				var result = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevation, tokenInfoPtr, tokenInfoLength, out returnLength);
				if (!result)
				{
					throw new ApplicationException("Unable to determine the current elevation.");
				}

				tokenElevation = (TOKEN_ELEVATION)Marshal.PtrToStructure(tokenInfoPtr, typeof(TOKEN_ELEVATION))!;
			}
			finally
			{
				if (tokenHandle != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(tokenInfoPtr);
					CloseHandle(tokenHandle);
				}
			}

			return tokenElevation.TokenIsElevated != 0;
		}
	}
	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool CloseHandle(IntPtr hObject);
	private const int TokenElevation = 20;
	private const uint TOKEN_QUERY = 0x0008;

	private enum TOKEN_INFORMATION_CLASS
	{
		TokenUser = 1,
		TokenGroups,
		TokenPrivileges,
		TokenOwner,
		TokenPrimaryGroup,
		TokenDefaultDacl,
		TokenSource,
		TokenType,
		TokenImpersonationLevel,
		TokenStatistics,
		TokenRestrictedSids,
		TokenSessionId,
		TokenGroupsAndPrivileges,
		TokenSessionReference,
		TokenSandBoxInert,
		TokenAuditPolicy,
		TokenOrigin,
		TokenElevationType,
		TokenLinkedToken,
		TokenElevation,
		TokenHasRestrictions,
		TokenAccessInformation,
		TokenVirtualizationAllowed,
		TokenVirtualizationEnabled,
		TokenIntegrityLevel,
		TokenUIAccess,
		TokenMandatoryPolicy,
		TokenLogonSid,
		MaxTokenInfoClass
	}
	

	[StructLayout(LayoutKind.Sequential)]
	private struct TOKEN_ELEVATION
	{
		public int TokenIsElevated;
	}

	
}
