namespace FitTech.WebComponents.Models;

public class Result
{
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = [];

    public static Result Success => new () { Succeeded = true };
    public static Result Failure(string[] errors) => new() { Errors = errors };
}

public class Result<T> : Result
{
    public T? Value { get; set; }

    public static new Result<T> Success(T result) => new() {Value = result, Succeeded = true};
    public static new Result<T> Failure(string[] errors) => (Result.Failure(errors) as Result<T>)!;
}
