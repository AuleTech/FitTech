using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Extensions;

internal static class ResultExtensions
{
    public static Result ToResult(this IdentityResult result) => new Result()
    {
        Succeeded = result.Succeeded, Errors = result.Errors.Select(x => x.Description).ToArray()
    };
}
