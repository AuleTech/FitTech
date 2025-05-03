using AuleTech.Core.Patterns;
using Cocona;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.GenerateOpenApiTypedClient;

public class GenerateOpenApiTypedClientCommand : ICommand<GenerateOpenApiTypedClientParams, Result>
{
    private readonly ILogger<GenerateOpenApiTypedClientCommand> _logger;

    public GenerateOpenApiTypedClientCommand(ILogger<GenerateOpenApiTypedClientCommand> logger)
    {
        _logger = logger;
    }

    [Command("GenerateOpenApiTypedClient")]
    public async Task<int> RunCommandAsync(GenerateOpenApiTypedClientParams commandParams, [FromService] CoconaAppContext context)
    {
        var result = await RunAsync(commandParams, context.CancellationToken);

        result.LogErrorsIfAny(_logger);

        return result.ToCliExitCode();
    }
    
    public Task<Result> RunAsync(GenerateOpenApiTypedClientParams commandParams, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
