using AuleTech.Core.Patterns;
using Cocona;

namespace DevopsCli.Core.Commands.Dotnet.Tests;

public class RunTestsCommandParams : ICommandParameterSet, IValidableParams
{
    public string ProjectPath { get; set; } = null!;
    
    private readonly string[] _supportedExtensions = [".csproj"];
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(ProjectPath))
        {
            return Result.Failure($"{nameof(ProjectPath)} cannot be empty");
        }

        var fileExtension = Path.GetExtension(ProjectPath);
        if(!_supportedExtensions.Contains(fileExtension))
        {
            return Result.Failure($"Extension('{fileExtension}') not supported");
        }
        
        return Result.Success;
    }
}
