using AuleTech.Core.Patterns.Result;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Extensions;

internal static class ResultExtensions
{
    public static Result ToResult(this IdentityResult result)
    {
        return new Result { Succeeded = result.Succeeded, Errors = result.Errors.Select(x => x.Description).ToArray() };
    }

    public static Result<T> ToTypedResult<T>(this Result result)
    {
        return new Result<T> { Errors = result.Errors, Succeeded = result.Succeeded };
    }
}
