using Cocona;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.Sample;

internal sealed class SampleCommand : ICommand<SampleCommandParams, CommandResult>
{
    private readonly ILogger<SampleCommand> _logger;

    public SampleCommand(ILogger<SampleCommand> logger)
    {
        _logger = logger;
    }

    public Task<CommandResult> RunAsync(SampleCommandParams commandParams, CancellationToken cancellation)
    {
        if (!commandParams.IsValid())
        {
            return Task.FromResult(CommandResult.Failed());
        }

        _logger.LogInformation("Running sample command with param: {Param}", commandParams.Param1);
        return Task.FromResult(CommandResult.Succeed());
    }

    [Command("sample")]
    public async Task<int> SampleAsync(SampleCommandParams commandParams, [FromService] CoconaAppContext context)
    {
        return await RunAsync(commandParams, context.CancellationToken);
    }
}
