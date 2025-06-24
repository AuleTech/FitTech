namespace AuleTech.Core.Patterns.CQRS;

public interface IValidator
{
    Result.Result Validate();
}
