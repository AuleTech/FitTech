using AuleTech.Core.Patterns;
using Cocona;

namespace DevopsCli.Core.Commands.GenerateOpenApiTypedClient;

public class GenerateOpenApiTypedClientParams : ICommandParameterSet, IValidableParams
{
    public string OpenApiJsonUrl { get; set; } = null!;
    public string OutputFolder { get; set; } = null!;
    public string Namespace { get; set; } = null!;
    
    
    public Result IsValid()
    {
        if (string.IsNullOrWhiteSpace(OpenApiJsonUrl))
        {
            return Result.Failure($"{nameof(OpenApiJsonUrl)} cannot be null");
        }
        
        if (string.IsNullOrWhiteSpace(OutputFolder))
        {
            return Result.Failure($"{nameof(OutputFolder)} cannot be null");
        }
        
        if (string.IsNullOrWhiteSpace(Namespace))
        {
            return Result.Failure($"{nameof(Namespace)} cannot be null");
        }
        
        return Result.Success;
    }
}
