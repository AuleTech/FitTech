﻿using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;

namespace DevopsCli.Core.Tools.Nswag;

internal interface INSwagTool : ITool
{
    Task<Result> GenerateCSharpClientAsync(string openApiJsonUrl
        , string outputFolder
        , string @namespace
        , CancellationToken cancellationToken
        , bool allowToolToSpecifyClientName = false
        , bool generatePublicDtoModels = true
        , string? rawOptions = null);
}
