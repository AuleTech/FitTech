namespace AuleTech.Core.Patterns.CQRS;

public static class ValidatorExtensions
{
    public static void ValidateStringNullOrEmpty(this string value, List<string> errors, string memberName)
    {
        value.ValidateGenericMember(() => string.IsNullOrWhiteSpace(value), errors, memberName);
    }

    public static void ValidateGenericMember<T>(this T value, Func<bool> condition, List<string> errors,
        string memberName)
    {
        if (condition())
        {
            errors.Add($"{memberName} is required");
        }
    }
}
