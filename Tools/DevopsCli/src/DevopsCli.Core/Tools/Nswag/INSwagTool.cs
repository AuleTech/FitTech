using AuleTech.Core.Patterns;

namespace DevopsCli.Core.Tools.Nswag;

internal interface INSwagTool : ITool
{
    Task<Result> GenerateCSharpClientAsync(string openApiJsonUrl
        , string outputFolder
        , string @namespace
        , bool generatePublicDtoModels
        , CancellationToken cancellationToken
        , bool allowToolToSpecifyClientName = false
        , string? rawOptions = null);
}
