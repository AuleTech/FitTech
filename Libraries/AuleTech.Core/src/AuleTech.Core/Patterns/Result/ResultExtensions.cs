﻿using AuleTech.Core.Processing.Runners;

namespace AuleTech.Core.Patterns.Result;

public static class ResultExtensions
{
    public static Result ToResult(this ProcessResult result)
    {
        if (result.Errored())
        {
            return Result.Failure(result.Output);
        }
        
        return Result.Success;
    }

    public static int ToCliExitCode(this Result result) => result.Succeeded ? 0 : 255;
}
