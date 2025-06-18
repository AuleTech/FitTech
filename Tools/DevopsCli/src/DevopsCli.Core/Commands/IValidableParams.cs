using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;

namespace DevopsCli.Core.Commands;

public interface IValidableParams
{
    Result Validate();
}
