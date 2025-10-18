using System.Net.Mail;

namespace AuleTech.Core.Patterns.CQRS.Validations;

public static partial class ValidatorExtensions
{
    public static void ValidateGenericMember<T>(this T value, Func<T,bool> condition, List<string> errors, string message)
    {
        if (!condition(value))
        {
            errors.Add(message);
        }
    }

    public static void ValidateNotEmpty(this Guid value, List<string> errors, string memberName)
    {
        value.ValidateGenericMember(x => x != Guid.Empty, errors, $"{memberName} is required");
    }

    public static void ValidateAge(this int age, List<string> errors, string memberName)
    {
        age.ValidateGenericMember(x => x is >= 16 and < 100, errors, $"{memberName} should be between 16 and 100");
    }

    public static void ValidateIsAdult(this DateOnly birth, List<string> errors)
    {
        birth.ValidateGenericMember(x => x <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-16)), errors, $"You need to be older than 16");
    }
}
