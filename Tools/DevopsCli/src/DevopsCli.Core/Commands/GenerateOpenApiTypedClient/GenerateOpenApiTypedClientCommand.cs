using AuleTech.Core.Patterns;
using Cocona;
using DevopsCli.Core.Tools.Nswag;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Commands.GenerateOpenApiTypedClient;

internal sealed class GenerateOpenApiTypedClientCommand : ICommand<GenerateOpenApiTypedClientParams, Result>
{
    private readonly ILogger<GenerateOpenApiTypedClientCommand> _logger;
    private readonly INSwagTool _nSwagTool;

    public GenerateOpenApiTypedClientCommand(ILogger<GenerateOpenApiTypedClientCommand> logger, INSwagTool nSwagTool)
    {
        _logger = logger;
        _nSwagTool = nSwagTool;
    }

    [Command("GenerateOpenApiTypedClient")]
    public async Task<int> RunCommandAsync(GenerateOpenApiTypedClientParams commandParams, [FromService] CoconaAppContext context)
    {
        var result = await RunAsync(commandParams, context.CancellationToken);

        result.LogErrorsIfAny(_logger);

        return result.ToCliExitCode();
    }
    
    public async Task<Result> RunAsync(GenerateOpenApiTypedClientParams commandParams, CancellationToken cancellationToken)
    {
        var validationResult = commandParams.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }
        
        return await _nSwagTool.GenerateCSharpClientAsync(commandParams.OpenApiJsonUrl, commandParams.OutputFolder,
            commandParams.Namespace, cancellationToken);
    }
}
