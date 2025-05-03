using AuleTech.Core.Patterns;
using Cocona;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.Sample;

internal sealed class SampleCommand : ICommand<SampleCommandParams, Result>
{
    private readonly ILogger<SampleCommand> _logger;

    public SampleCommand(ILogger<SampleCommand> logger)
    {
        _logger = logger;
    }

    public Task<Result> RunAsync(SampleCommandParams commandParams, CancellationToken cancellation)
    {
        var isValidResult = commandParams.IsValid(); 
        
        if (!isValidResult.Succeeded)
        {
            return Task.FromResult(Result.Failure());
        }

        _logger.LogInformation("Running sample command with param: {Param}", commandParams.Param1);
        return Task.FromResult(Result.Failure());
    }

    [Command("sample")]
    public async Task<int> RunCommandAsync(SampleCommandParams commandParams, [FromService] CoconaAppContext context)
    {
        var result = await RunAsync(commandParams, context.CancellationToken);

        result.LogErrorsIfAny(_logger);

        return result.ToCliExitCode();
    }
}
