using AuleTech.Core.Patterns.Result;
using Refit;

namespace AuleTech.Core.Extensions.Refit;

public static class ApiResponseExtensions
{
    extension(IApiResponse response)
    {
        public Result ToResult()
        {
            if (response.IsSuccessful)
            {
                return Result.Success;
            }

            return Result.Failure(response.Error?.Content ?? "Something went wrong, please try later");
        }
    }

    extension<T>(IApiResponse<T> response)
    {
        public Result<T> ToResult()
        {
            if (response.IsSuccessful)
            {
                return response.Content;
            }

            return Result<T>.Failure(response.Error?.Content ?? "Something went wrong, please try later");
        }

        public Result<TOut> ToFailedResult<TOut>() => Result<TOut>.Failure(response.Error?.Content ?? "Something went wrong, please try later");
    }
}
