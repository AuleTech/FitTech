using AuleTech.Core.Patterns.Result;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Dotnet.Build;
using DevopsCli.Core.Commands.Dotnet.Restore;
using DevopsCli.Core.Commands.Dotnet.Test;
using DevopsCli.Core.Commands.Dotnet.Workloads;
using DevopsCli.Core.Tools.Node;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;

[GitHubActions(
    "Continuous Integration",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.PullRequest })]
internal class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] private readonly Solution Solution;

    private Target InstallDependencies => _ => _
        .Executes(() =>
        {
            var nodeTool = PacDependencyInjection.Default.Get<INodeTool>();
            var result = nodeTool.NpmInstallAsync("tailwindcss @tailwindcss/cli", CancellationToken.None).GetAwaiter()
                .GetResult();
            result.ThrowIfFailed();

            result = nodeTool.NpmInstallAsync(string.Empty, CancellationToken.None,
                    workingDir: Solution.src._Presentation.FitTech_WebComponents.Directory, isGlobal: false)
                .GetAwaiter().GetResult();

            result.ThrowIfFailed();

            var restoreWorkloadCommand = PacDependencyInjection.Default.Get<ICommand<WorkloadsCommandParams, Result>>();
            result = restoreWorkloadCommand
                .RunAsync(
                    new WorkloadsCommandParams
                    {
                        Project = Solution.src._Presentation.Client.FitTech_Client_Mobile,
                        RunAsAdministrator = IsLocalBuild
                    },
                    CancellationToken.None).GetAwaiter().GetResult();

            result.ThrowIfFailed();
        });

    private Target Restore => _ => _
        .DependsOn(InstallDependencies)
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var command = PacDependencyInjection.Default.Get<ICommand<RestoreCommandParams, Result>>();

            var result =
                command.RunAsync(
                    new RestoreCommandParams { SolutionPath = Solution },
                    cts.Token).GetAwaiter().GetResult();

            result.ThrowIfFailed();
        });

    private Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var command = PacDependencyInjection.Default.Get<ICommand<BuildCommandParams, Result>>();

            Result result;
            do //TODO: There is a bug where the .gz file is not generated on the first compilation so we need to retry. We need to add a proper retry way with Polly.
            {
                result =
                    command.RunAsync(
                            new BuildCommandParams { SolutionPath = Solution },
                            cts.Token)
                        .GetAwaiter().GetResult();
            } while (!result.Succeeded);
        });

    private Target UnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var command = PacDependencyInjection.Default.Get<ICommand<RunTestsCommandParams, Result>>();

            var result =
                command.RunAsync(
                        new RunTestsCommandParams
                        {
                            ProjectPath = Solution.src._APIs.FitTech.Tests.FitTech_API_UnitTests.Path
                        },
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
                        new RunTestsCommandParams
                        {
                            ProjectPath = Solution.src._APIs.FitTech.Tests.FitTech_API_IntegrationTests.Path
                        },
                        cts.Token)
                    .GetAwaiter().GetResult();

            result.ThrowIfFailed();
        });

    public static int Main()
    {
        return Execute<Build>(x => x.UnitTests, x => x.IntegrationTests);
    }
}
