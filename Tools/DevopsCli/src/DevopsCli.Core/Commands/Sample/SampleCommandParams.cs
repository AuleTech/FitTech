using AuleTech.Core.Patterns;
using Cocona;

namespace DevopsCli.Core.Commands.Sample;

public class SampleCommandParams : ICommandParameterSet, IValidableParams
{
    public string Param1 { get; set; } = null!;

    public Result IsValid()
    {
        if (string.IsNullOrWhiteSpace(Param1))
        {
            return Result.Failure($"{nameof(Param1)} cannot be null");
        }
        
        return Result.Success;
    }
}
