using AuleTech.Core.Patterns;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Dotnet.Build;
using DevopsCli.Core.Commands.Dotnet.Restore;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

[GitHubActions(
    "FitTech Pr pipeline",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.PullRequest },
    InvokedTargets = new[] { nameof(Compile) })]
class Build : NukeBuild
{
    [Solution(GenerateProjects = true)] readonly Solution Solution;
    
    public static int Main()
    {
        var result = Execute<Build>(x => x.Compile);

        return result;
    }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Restore => _ => _
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var command = PacDependencyInjection.Default.Get<ICommand<RestoreCommandParams, Result>>();

            var result =
                command.RunAsync(
                    new RestoreCommandParams() { SolutionPath = Solution }, cts.Token).GetAwaiter().GetResult();

            result.ThrowIfFailed();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var command = PacDependencyInjection.Default.Get<ICommand<BuildCommandParams, Result>>();

            var result =
                command.RunAsync(
                    new BuildCommandParams() { SolutionPath = Solution }, cts.Token).GetAwaiter().GetResult();

            result.ThrowIfFailed();
        });
}
