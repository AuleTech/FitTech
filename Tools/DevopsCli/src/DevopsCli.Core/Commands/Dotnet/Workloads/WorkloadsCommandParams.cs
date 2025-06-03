using AuleTech.Core.Patterns;
using Cocona;

namespace DevopsCli.Core.Commands.Dotnet.Workloads;

public class WorkloadsCommandParams : ICommandParameterSet
{
    public string Project { get; set; } = null!;
    public bool RunAsAdministrator { get; set; }
}
