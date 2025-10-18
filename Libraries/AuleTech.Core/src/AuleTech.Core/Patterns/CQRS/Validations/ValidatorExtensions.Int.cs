namespace AuleTech.Core.Patterns.CQRS.Validations;

public static partial class ValidatorExtensions
{
    public static void ValidateBetween(this int value, int start, int end, List<string> errors, string memberName)
    {
        value.ValidateGenericMember(x => x >= start && x <= end, errors, $"{memberName} need to be between [{start},{end}]");
    }
}
