namespace DevopsCli.Core.Commands;

public interface ICommand<TCommandParams, TResult> where TResult : CommandResult
    where TCommandParams : class
{
    Task<TResult> RunAsync(TCommandParams commandParams, CancellationToken cancellation);
}
