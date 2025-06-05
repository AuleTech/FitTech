using Microsoft.Extensions.Logging;

namespace AuleTech.Core.Patterns;

public class Result
{
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = [];

    public static Result Success => new() { Succeeded = true };

    public static Result Failure(string error)
    {
        return Failure([error]);
    }

    public static Result Failure(string[] errors)
    {
        return new Result { Errors = errors };
    }

    public static Result Failure()
    {
        return new Result();
    }

    //TODO: Think a better way
    public void LogErrorsIfAny(ILogger logger)
    {
        if (!Succeeded)
        {
            logger.LogError("Execution exited with errors: [{Errors}]", ToErrorString());   
        }
    }

    public void ThrowIfFailed()
    {
        if (!Succeeded)
        {
            throw new Exception($"Error during execution: {ToErrorString()}");
        }
    }

    private string ToErrorString() => string.Join(',', Errors.Select(x => $"'{x}'"));
}

public class Result<T> : Result
{
    public T? Value { get; set; }

    public Result<TOut> MapFailure<TOut>()
    {
        return Result<TOut>.Failure(Errors);
    }

    public Result<TOut> Map<TOut>(Func<T?, TOut?> mapFunc)
    {
        return new Result<TOut> { Errors = Errors, Succeeded = Succeeded, Value = mapFunc(Value) };
    }

    public static new Result<T> Success(T result)
    {
        return new Result<T> { Value = result, Succeeded = true };
    }

    public static new Result<T> Failure(string error)
    {
        return Failure([error]);
    }

    public static new Result<T> Failure(string[] errors)
    {
        return new Result<T>()
        {
            Succeeded = false,
            Errors = errors
        };
    }

    public static new Result<T> Failure()
    {
        return new Result<T>();
    }

    public static implicit operator Result<T>(T result)
    {
        return Success(result);
    }
}
