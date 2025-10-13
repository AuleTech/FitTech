using System.Runtime.CompilerServices;

namespace FitTech.Application.Extensions;

internal static class ErrorMessageExtensions
{
    public static string RequiredErrorMessage(this object value, [CallerMemberName] string propertyName = "")
    {
        return $"{propertyName} is required";
    }
}
