using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AuleTech.Core.Patterns.CQRS;

public static class ValidatorExtensions
{
    public static void ValidateStringNullOrEmpty(this string value, List<string> errors, string memberName)
    {
        value.ValidateGenericMember(() => string.IsNullOrWhiteSpace(value), errors, $"{memberName} is required");
    }

    public static void ValidateGenericMember<T>(this T value, Func<bool> condition, List<string> errors, string message)
    {
        if (condition())
        {
            errors.Add(message);
        }
    }

    public static void ValidateEmail(this string value, List<string> errors, string memberName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{memberName} is required");
            return;
        }
        
        value.ValidateGenericMember(() =>
            {
                try
                {
                    _ = new MailAddress(value).Address;
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }, errors, $"{memberName} invalid");
    }

    public static void ValidateUrl(this string value, List<string> errors, string memberName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{memberName} is required");
            return;
        }
        
        
        value.ValidateGenericMember(() => !(Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute) && value.Contains('.')), errors, $"{memberName} invalid format");
    }
}
