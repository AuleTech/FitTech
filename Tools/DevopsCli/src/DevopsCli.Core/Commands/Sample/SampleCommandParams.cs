using Cocona;

namespace DevopsCli.Core.Commands.Sample;

public class SampleCommandParams : ICommandParameterSet, IValidableParams
{
    public string Param1 { get; set; } = null!;

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Param1);
    }
}
