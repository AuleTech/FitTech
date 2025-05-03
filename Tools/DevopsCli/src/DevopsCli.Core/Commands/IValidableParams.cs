using AuleTech.Core.Patterns;

namespace DevopsCli.Core.Commands;

public interface IValidableParams
{
    Result IsValid();
}
