using Cocona;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.Sample;

public class SampleCommand : ICommand<SampleCommandParams, CommandResult>
{
    private ILogger<SampleCommand> _logger;

    public SampleCommand(ILogger<SampleCommand> logger)
    {
        _logger = logger;
    }

    [Command("sample")]
    public async Task<int> SampleAsync(SampleCommandParams commandParams, [FromService] CoconaAppContext context)
    {
        return await RunAsync(commandParams, context.CancellationToken);
    }

    public Task<CommandResult> RunAsync(SampleCommandParams commandParams, CancellationToken cancellation)
    {
        _logger.LogInformation("Running sample command with param: {Param}", commandParams.Param1);
        return Task.FromResult(CommandResult.Succeed());
    }
}
