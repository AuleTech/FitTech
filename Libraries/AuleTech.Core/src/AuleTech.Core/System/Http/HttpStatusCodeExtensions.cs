using System.Net;
using System.Runtime.CompilerServices;

namespace AuleTech.Core.System.Http
{
	public static class HttpStatusCodeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSuccess(this HttpStatusCode statusCode)
		{
			var value = (int) statusCode;
			return value is >= 200 and <= 299;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsError(this HttpStatusCode statusCode)
		{
			var value = (int)statusCode;
			return value >= 400;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNotSuccessful(this HttpStatusCode statusCode
		                                        , string? messageOnError = null)
		{
			if (!statusCode.IsSuccess())
			{
				throw new InvalidOperationException(messageOnError ?? "The last operation failed");
			}
		}
	}
}