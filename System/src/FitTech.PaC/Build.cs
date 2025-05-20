using AuleTech.Core.Patterns;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Dotnet.Build;
using DevopsCli.Core.Commands.Dotnet.Restore;
using DevopsCli.Core.Commands.Dotnet.Tests;
using DevopsCli.Core.Tools;
using DevopsCli.Core.Tools.Node;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Npm;

[GitHubActions(
    "Continous Integration",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.PullRequest })]
class Build : NukeBuild
{
    //TODO: Check async execution
    [Solution(GenerateProjects = true)] readonly Solution Solution;

    public static int Main() => Execute<Build>(x => x.UnitTests, x => x.IntegrationTests);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    private Target InstallDependencies => _ => _
        .Executes(() =>
        {
            var nodeTool = PacDependencyInjection.Default.Get<INodeTool>();
            var result = nodeTool.NpmInstallAsync("tailwindcss @tailwindcss/cli", CancellationToken.None).GetAwaiter().GetResult();
            result.ThrowIfFailed();

            result = nodeTool.NpmInstallAsync(string.Empty, CancellationToken.None, 
                    workingDir: Solution.src._Presentation.FitTech_WebComponents.Directory, isGlobal: false)
                .GetAwaiter().GetResult();
            
            result.ThrowIfFailed();
        });
    
    Target Restore => _ => _
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var command = PacDependencyInjection.Default.Get<ICommand<RestoreCommandParams, Result>>();

            var result =
                command.RunAsync(
                    new RestoreCommandParams() { SolutionPath = Solution },
                    cts.Token).GetAwaiter().GetResult();

            result.ThrowIfFailed();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(InstallDependencies)
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var command = PacDependencyInjection.Default.Get<ICommand<BuildCommandParams, Result>>();

            var result =
                command.RunAsync(
                        new BuildCommandParams() { SolutionPath = Solution },
                        cts.Token)
                    .GetAwaiter().GetResult();

            result.ThrowIfFailed();
        });

    private Target UnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var command = PacDependencyInjection.Default.Get<ICommand<RunTestsCommandParams, Result>>();

            var result =
                command.RunAsync(
                        new RunTestsCommandParams() { ProjectPath = Solution.src._APIs.FitTech.Tests.FitTech_API_UnitTests.Path },
                        cts.Token)
                    .GetAwaiter().GetResult();

            result.ThrowIfFailed();
        });
    
    private Target IntegrationTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var command = PacDependencyInjection.Default.Get<ICommand<RunTestsCommandParams, Result>>();

            var result =
                command.RunAsync(
                        new RunTestsCommandParams() { ProjectPath = Solution.src._APIs.FitTech.Tests.FitTech_API_IntegrationTests.Path },
                        cts.Token)
                    .GetAwaiter().GetResult();

            Serilog.Log.Logger.Information($"Resutado: {result.Succeeded} : [{string.Join(',',result.Errors.Select(x => x))}]");
            
            result.ThrowIfFailed();
        });
}
