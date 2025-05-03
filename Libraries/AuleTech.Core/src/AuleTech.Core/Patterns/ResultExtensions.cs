using AuleTech.Core.Processing.Runners;

namespace AuleTech.Core.Patterns;

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
}
