
using AuleTech.Core.Patterns;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Dotnet.Build;
using DevopsCli.Core.Commands.Dotnet.Restore;
using Nuke.Common;
using Nuke.Common.ProjectModel;

class Build : NukeBuild
{
 
    [Solution]
    readonly Solution Solution;  
    
    public static int Main()
    {
      return Execute<Build>(x => x.Compile);  
    } 

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    Target Restore => _ => _
        .Executes(() =>
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                var command = PacDependencyInjection.Default.Get<ICommand<RestoreCommandParams, Result>>();

                var result =
                     command.RunAsync(
                        new RestoreCommandParams() { SolutionPath = Solution }, cts.Token).GetAwaiter().GetResult();

                return result.ToCliExitCode();
            }
            catch (Exception ex)
            {
                Serilog.Log.Logger.Error(ex.Message);
                return Result.Failure().ToCliExitCode();
            }
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                var command = PacDependencyInjection.Default.Get<ICommand<BuildCommandParams, Result>>();

                var result =
                     command.RunAsync(
                        new BuildCommandParams() { SolutionPath = Solution }, cts.Token).GetAwaiter().GetResult();
                
            }
            catch (Exception ex)
            {
                Serilog.Log.Logger.Error(ex.Message);
            }
        });

}
