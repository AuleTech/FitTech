using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;

namespace DevopsCli.Core.Commands;

public interface ICommand<TCommandParams, TResult> where TResult : Result
    where TCommandParams : class
{
    Task<TResult> RunAsync(TCommandParams commandParams, CancellationToken cancellationToken);
}
