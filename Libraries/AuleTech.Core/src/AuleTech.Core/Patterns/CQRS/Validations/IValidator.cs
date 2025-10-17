namespace AuleTech.Core.Patterns.CQRS.Validations;

public interface IValidator
{
    Result.Result Validate();
}
