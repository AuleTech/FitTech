using AuleTech.Core.Patterns;
using AuleTech.Core.Processing.Runners;
using AuleTech.Core.System.IO.FileSystem;
using Microsoft.Extensions.Logging;

namespace DevopsCli.Core.Tools.Nswag;

internal sealed class NSwagTool : INSwagTool
{
    private readonly IInstaller<NSwagTool> _installer;
    private readonly ILogger<NSwagTool> _logger;
    private readonly IProcessRunner _processRunner;
    private readonly ISystemIo _systemIo;

    public NSwagTool(IInstaller<NSwagTool> installer, IProcessRunner processRunner, ILogger<NSwagTool> logger,
        ISystemIo systemIo)
    {
        _installer = installer;
        _processRunner = processRunner;
        _logger = logger;
        _systemIo = systemIo;
    }

    public async Task<Result> GenerateCSharpClientAsync(string openApiJsonUrl
        , string outputFolder
        , string @namespace
        , bool generatePublicDtoModels
        , CancellationToken cancellationToken
        , bool allowToolToSpecifyClientName = false
        , string? rawOptions = null)
    {
        var installResult = await _installer.InstallAsync(cancellationToken);

        if (!installResult.Succeeded)
        {
            return installResult;
        }
        
        _systemIo.Directory.CreateDirectory(outputFolder, false, false);

        return await GenerateAsync();

        async Task<Result> GenerateAsync()
        {
            try
            {
                var outputFile = $"{outputFolder}/Proxy.generated.cs";
                var arguments =
                    $"openapi2csclient /input:\"{openApiJsonUrl}\" /namespace:{@namespace} /output:\"{outputFile}\" /clientClassAccessModifier:internal /typeAccessModifier:{(generatePublicDtoModels ? "public" : "internal")} {(allowToolToSpecifyClientName ? string.Empty : "/classname:Proxy")} /generateOptionalPropertiesAsNullable:true /generateNullableReferenceTypes:true {rawOptions}";
                var result = await _processRunner.RunAsync(new AuleTechProcessStartInfo("nswag"
                        , arguments)
                    , cancellationToken);
                var success = result.Output.Contains("Code has been successfully");
                if (success)
                {
                    await InternalizeApiException(outputFile);
                }

                return success ? Result.Success : Result.Failure();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Failure(e.Message);
            }
        }

        async Task InternalizeApiException(string outputFile)
        {
            var txt = await _systemIo.File.ReadAllTextAsync(outputFile, cancellationToken);
            txt = txt.Replace("public partial class ApiException", "internal partial class ApiException");
            await _systemIo.File.WriteAllTextAsync(outputFile, txt, cancellationToken);
        }
    }
}
