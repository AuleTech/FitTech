using System.Diagnostics;

namespace AuleTech.Core.Resiliency;

public partial class ResilientOperations
{
    public async Task ThrottleAsync<TItem>(IEnumerable<TItem> itemsCollection
        , int maxConcurrentTasks
        , Func<TItem, CancellationToken, Task> payloadActionTask
        , CancellationToken cancellationToken = default)
    {
        if (payloadActionTask == null)
        {
            throw new ArgumentNullException(nameof(payloadActionTask));
        }

        await ThrottleAsync(itemsCollection
            , maxConcurrentTasks
            , async (item
                , ct) =>
            {
                await payloadActionTask(item, ct);
                return true;
            }
            , cancellationToken);
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="itemsCollection"></param>
    /// <param name="maxConcurrentTasks">if the debugger is attached this is automatically set to 1</param>
    /// <param name="payloadActionTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<IEnumerable<TResult>> ThrottleAsync<TItem, TResult>(IEnumerable<TItem> itemsCollection
        , int maxConcurrentTasks
        , Func<TItem, CancellationToken,
                Task<TResult>>
            payloadActionTask
        , CancellationToken cancellationToken =
            default)
    {
        if (itemsCollection == null)
        {
            throw new ArgumentNullException(nameof(itemsCollection));
        }

        if (payloadActionTask == null)
        {
            throw new ArgumentNullException(nameof(payloadActionTask));
        }

        if (maxConcurrentTasks <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxConcurrentTasks));
        }

        if (Debugger.IsAttached)
        {
            maxConcurrentTasks = 1;
        }

        var itemsList = itemsCollection.ToArray();

        return await ExecuteAsync();


        async Task<TResult[]> ExecuteAsync()
        {
            var result = Array.Empty<TResult>();
            if (itemsList.Any())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pendingTasks = new List<Task<TResult>>(maxConcurrentTasks);
                var results = new List<Task<TResult>>(itemsList.Length);
                foreach (var input in itemsList)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (pendingTasks.Count >= maxConcurrentTasks)
                    {
                        await Task.WhenAny(pendingTasks);
                        pendingTasks.RemoveAll(t => t.IsCompleted);
                    }

                    var actionTask = payloadActionTask(input, cancellationToken);
                    results.Add(actionTask);
                    pendingTasks.Add(actionTask);
                }

                cancellationToken.ThrowIfCancellationRequested();
                result = await Task.WhenAll(results);
            }

            return result;
        }
    }
}
