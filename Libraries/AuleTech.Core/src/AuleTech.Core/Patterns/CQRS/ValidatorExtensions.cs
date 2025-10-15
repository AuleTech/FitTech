using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AuleTech.Core.Patterns.CQRS;

public static class ValidatorExtensions
{
    public static void ValidateStringNullOrEmpty(this string value, List<string> errors, string memberName)
    {
        value.ValidateGenericMember(string.IsNullOrWhiteSpace, errors, $"{memberName} is required");
    }

    public static void ValidateGenericMember<T>(this T value, Func<T,bool> condition, List<string> errors, string message)
    {
        if (condition(value))
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
        
        value.ValidateGenericMember(x =>
            {
                try
                {
                    _ = new MailAddress(x).Address;
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
        
        
        value.ValidateGenericMember(x => !(Uri.IsWellFormedUriString(x, UriKind.RelativeOrAbsolute) && value.Contains('.')), errors, $"{memberName} invalid format");
    }

    public static void ValidateNotEmpty(this Guid value, List<string> errors, string memberName)
    {
        value.ValidateGenericMember(x => x == Guid.Empty, errors, $"{memberName} is required");
    }
}
