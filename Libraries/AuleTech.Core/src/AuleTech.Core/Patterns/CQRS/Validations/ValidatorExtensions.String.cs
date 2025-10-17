using System.Net.Mail;

namespace AuleTech.Core.Patterns.CQRS.Validations;

public static partial class ValidatorExtensions
{
    public static void ValidateStringNullOrEmpty(this string value, List<string> errors, string memberName)
    {
        value.ValidateGenericMember(x => !string.IsNullOrWhiteSpace(x), errors, $"{memberName} is required");
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
                    return true;
                }
                catch (Exception)
                {
                    return false;
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
        
        
        value.ValidateGenericMember(x => Uri.IsWellFormedUriString(x, UriKind.RelativeOrAbsolute) && value.Contains('.'), errors, $"{memberName} invalid format");
    }
}
