using System.Diagnostics;

namespace AuleTech.Core.Resiliency;

[DebuggerStepThrough]
public partial class ResilientOperations
{
    public static readonly ResilientOperations Default = new();

    private ResilientOperations() { }

    public void RetryIfNeeded(Action<int> payloadAction
        , int maxAttempts = 2
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        RetryIfNeededAsync(i =>
                {
                    payloadAction(i);
                    return Task.CompletedTask;
                }
                , maxAttempts
                , waitBetweenAttempts
                , onException
                , customExceptionOnMaxAttemptsWithoutException
                , cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    public async Task RetryIfNeededAsync(Func<int, Task> payloadAction
        , int maxAttempts = 2
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        await RetryIfNeededAsync(async attempt =>
            {
                await payloadAction(attempt);
                return true;
            }
            , maxAttempts
            , waitBetweenAttempts
            , onException
            , customExceptionOnMaxAttemptsWithoutException is not null
                ? _ => customExceptionOnMaxAttemptsWithoutException()
                : null
            , cancellationToken);
    }

    public TResult RetryIfNeeded<TResult>(Func<int, TResult> payloadAction
        , int maxAttempts = 2
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(i => Task.FromResult(payloadAction(i))
                , maxAttempts
                , waitBetweenAttempts
                , onException
                , customExceptionOnMaxAttemptsWithoutException
                , cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    public TResult RetryIfNeeded<TResult>(Func<int, TResult> payloadAction
        , TimeSpan timeout
        , TimeSpan waitBetweenAttempts
        , Func<TResult, bool> mustRetry
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(i => Task.FromResult(payloadAction(i))
                , timeout
                , waitBetweenAttempts
                , mustRetry
                , onException
                , customExceptionOnMaxAttemptsWithoutException
                , cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    public void RetryIfNeeded(Action<int> payloadAction
        , TimeSpan timeout
        , TimeSpan waitBetweenAttempts
        , Action<Exception>? onException = null
        , Func<Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        RetryIfNeeded(i =>
            {
                payloadAction(i);
                return true;
            }
            , timeout
            , waitBetweenAttempts
            , onException
            , customExceptionOnMaxAttemptsWithoutException is not null
                ? _ => customExceptionOnMaxAttemptsWithoutException()
                : null
            , cancellationToken);
    }

    public TResult RetryIfNeeded<TResult>(Func<int, TResult> payloadAction
        , TimeSpan timeout
        , TimeSpan waitBetweenAttempts
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(i => Task.FromResult(payloadAction(i))
                , timeout
                , waitBetweenAttempts
                , onException
                , customExceptionOnMaxAttemptsWithoutException
                , cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    public Task<TResult> RetryIfNeededAsync<TResult>(Func<int, Task<TResult>> payloadAction
        , TimeSpan timeout
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        waitBetweenAttempts ??= TimeSpan.Zero;
        return RetryIfNeededAsync(payloadAction
            , timeout
            , waitBetweenAttempts.Value
            , r => false
            , onException
            , customExceptionOnMaxAttemptsWithoutException
            , cancellationToken);
    }

    public Task<TResult> RetryIfNeededAsync<TResult>(Func<int, Task<TResult>> payloadAction
        , TimeSpan timeout
        , TimeSpan waitBetweenAttempts
        , Func<TResult, bool> mustRetry
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(payloadAction
            , x => Task.FromResult(mustRetry(x))
            , null
            , timeout
            , waitBetweenAttempts
            , cancellationToken: cancellationToken
            , customExceptionOnMaxAttemptsWithoutException: customExceptionOnMaxAttemptsWithoutException
            , onException: onException);
    }

    public Task<TResult> RetryIfNeededAsync<TResult>(Func<int, Task<TResult>> payloadAction
        , TimeSpan timeout
        , Func<TResult, bool> mustRetry
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(payloadAction
            , timeout
            , TimeSpan.Zero
            , mustRetry
            , onException
            , customExceptionOnMaxAttemptsWithoutException
            , cancellationToken);
    }

    public async Task RetryIfNeededAsync(Func<int, Task> payloadAction
        , TimeSpan timeout
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        waitBetweenAttempts ??= TimeSpan.Zero;
        await RetryIfNeededAsync(async attempt =>
            {
                await payloadAction(attempt);
                return true;
            }
            , timeout
            , waitBetweenAttempts.Value
            , onException
            , _ => customExceptionOnMaxAttemptsWithoutException!()
            , cancellationToken);
    }

    public Task<TResult> RetryIfNeededAsync<TResult>(Func<int, Task<TResult>> payloadAction
        , int maxAttempts = 2
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(
            payloadAction
            , r => false
            , maxAttempts
            , waitBetweenAttempts
            , onException
            , customExceptionOnMaxAttemptsWithoutException
            , cancellationToken
        );
    }

    public TResult RetryIfNeeded<TResult>(Func<int, TResult> payloadAction
        , Func<TResult, bool> mustRetry
        , int maxAttempts = 2
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(i => Task.FromResult(payloadAction(i))
                , mustRetry
                , maxAttempts
                , waitBetweenAttempts
                , onException
                , customExceptionOnMaxAttemptsWithoutException
                , cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    public Task<TResult> RetryIfNeededAsync<TResult>(Func<int, Task<TResult>> payloadAction
        , Func<TResult, Task<bool>> mustRetryAsync
        , int maxAttempts = 2
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(
            payloadAction
            , mustRetryAsync
            , maxAttempts
            , null
            , waitBetweenAttempts
            , onException
            , customExceptionOnMaxAttemptsWithoutException
            , cancellationToken
        );
    }

    public Task<TResult> RetryIfNeededAsync<TResult>(Func<int, Task<TResult>> payloadAction
        , Func<TResult, bool> mustRetry
        , int maxAttempts = 2
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        return RetryIfNeededAsync(
            payloadAction
            , x => Task.FromResult(mustRetry(x))
            , maxAttempts
            , null
            , waitBetweenAttempts
            , onException
            , customExceptionOnMaxAttemptsWithoutException
            , cancellationToken
        );
    }

    private async Task<TResult> RetryIfNeededAsync<TResult>(Func<int, Task<TResult>> payloadAction
        , Func<TResult, Task<bool>> mustRetryAsync
        , int? maxAttempts = null
        , TimeSpan? timeout = null
        , TimeSpan? waitBetweenAttempts = null
        , Action<Exception>? onException = null
        , Func<TResult, Exception>? customExceptionOnMaxAttemptsWithoutException = null
        , CancellationToken cancellationToken = default)
    {
        if (mustRetryAsync == null)
        {
            throw new ArgumentNullException(nameof(mustRetryAsync));
        }

        if (
            ((maxAttempts == null || maxAttempts == 0) && (timeout == null || timeout == TimeSpan.Zero))
            || (maxAttempts != null && maxAttempts != 0 && timeout != null && timeout > TimeSpan.Zero)
        )
        {
            throw new ArgumentException(
                $"You need to specify a valid value for one and only one of {nameof(maxAttempts)} or {nameof(timeout)}");
        }

        var succeeded = false;
        var result = default(TResult);

        var byTimeout = timeout != null && timeout > TimeSpan.Zero;
        var started = DateTime.UtcNow;
        if (byTimeout)
        {
            maxAttempts = int.MaxValue;
        }

        var totalAttempts = maxAttempts;
        while (!succeeded)
        {
            ThrowIfCancelled();
            try
            {
                result = await payloadAction(maxAttempts!.Value);
                succeeded = !await mustRetryAsync(result);
                --maxAttempts;
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);

                if ((byTimeout && ItTimedOut()) || (!byTimeout && --maxAttempts == 0))
                {
                    throw;
                }
            }

            if (!succeeded)
            {
                if (CanThrowCustomException() && ((byTimeout && ItTimedOut()) || maxAttempts == 0))
                {
                    throw customExceptionOnMaxAttemptsWithoutException!(result!);
                }

                if (byTimeout)
                {
                    if (ItTimedOut())
                    {
                        throw new TimeoutException(
                            $"Could not complete operation in the time given({timeout})");
                    }
                }
                else
                {
                    if (maxAttempts == 0)
                    {
                        throw new InvalidOperationException(
                            $"Could not complete operation in the given number of attempts({totalAttempts})");
                    }
                }

                ThrowIfCancelled();
                if (waitBetweenAttempts.HasValue)
                {
                    await Task.Delay(waitBetweenAttempts.Value, cancellationToken);
                }
                else
                {
                    await Task.Yield();
                }
            }
        }

        return result!;

        void ThrowIfCancelled()
        {
            if (cancellationToken != default && cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }

        bool ItTimedOut()
        {
            return DateTime.UtcNow.Subtract(started) > timeout!.Value;
        }

        bool CanThrowCustomException()
        {
            return customExceptionOnMaxAttemptsWithoutException is not null;
        }
    }
}
