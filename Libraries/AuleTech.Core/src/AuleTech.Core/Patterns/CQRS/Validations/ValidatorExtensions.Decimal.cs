namespace AuleTech.Core.Patterns.CQRS.Validations;

public static partial class ValidatorExtensions
{
    public static void ValidateIsBetween(this decimal value, List<string> errors, string memberName, decimal min, decimal max = decimal.MaxValue)
    {
        value.ValidateGenericMember(x => x >= min && x <= max, errors, $"{memberName} should be between'[{min},{max}]'");
    }
}
