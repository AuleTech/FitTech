using AuleTech.Core.Patterns;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Extensions;

internal static class ResultExtensions
{
    public static Result ToResult(this IdentityResult result) => new Result()
    {
        Succeeded = result.Succeeded, Errors = result.Errors.Select(x => x.Description).ToArray()
    };
    
    public static Result<T> ToTypedResult<T>(this Result result) => new ()
    {
        Errors = result.Errors, Succeeded = result.Succeeded
    };
}
