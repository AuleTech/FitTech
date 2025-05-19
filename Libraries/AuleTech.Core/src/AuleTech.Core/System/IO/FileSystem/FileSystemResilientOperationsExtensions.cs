using AuleTech.Core.Resiliency;

namespace AuleTech.Core.System.IO.FileSystem;

internal static class FileSystemResilientOperationsExtensions
{
    public static void FileSystemRetry(this ResilientOperations src
        , Action<int> payload
        , CancellationToken cancellationToken = default)
    {
        src.FileSystemRetry(_ =>
            {
                payload(_);
                return true;
            }
            , cancellationToken);
    }

    public static TResult FileSystemRetry<TResult>(this ResilientOperations src
        , Func<int, TResult> payload
        , CancellationToken cancellationToken = default)
    {
        return src.FileSystemRetryAsync(_ => Task.FromResult(payload(_))
                , cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    public static Task FileSystemRetryAsync(this ResilientOperations src
        , Func<int, Task> payload
        , CancellationToken cancellationToken = default)
    {
        return src.FileSystemRetryAsync(async p =>
            {
                await payload(p);
                return true;
            }
            , cancellationToken);
    }

    public static Task<TResult> FileSystemRetryAsync<TResult>(this ResilientOperations src
        , Func<int, Task<TResult>> payload
        , CancellationToken cancellationToken = default)
    {
        return src.RetryIfNeededAsync(payload
            , TimeSpan.FromSeconds(90) //Network File System could be under pressure
            , TimeSpan.FromMilliseconds(250)
            , cancellationToken: cancellationToken);
    }
}
