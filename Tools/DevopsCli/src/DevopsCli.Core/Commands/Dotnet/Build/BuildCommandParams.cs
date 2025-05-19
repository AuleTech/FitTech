using AuleTech.Core.Patterns;
using Cocona;

namespace DevopsCli.Core.Commands.Dotnet.Build;

public class BuildCommandParams : ICommandParameterSet, IValidableParams
{
    public string SolutionPath { get; set; } = null!;

    private readonly string[] _supportedExtensions = [".csproj", ".sln"];

    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(SolutionPath))
        {
            return Result.Failure($"{nameof(SolutionPath)} cannot be empty");
        }

        var fileExtension = Path.GetExtension(SolutionPath);
        if (!_supportedExtensions.Contains(fileExtension))
        {
            return Result.Failure($"Extension('{fileExtension}') not supported");
        }

        return Result.Success;
    }
}
