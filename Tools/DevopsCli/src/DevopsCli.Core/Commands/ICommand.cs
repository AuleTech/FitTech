using AuleTech.Core.Patterns;

namespace DevopsCli.Core.Commands;

public interface ICommand<TCommandParams, TResult> where TResult : Result
    where TCommandParams : class
{
    Task<TResult> RunAsync(TCommandParams commandParams, CancellationToken cancellation);
}
