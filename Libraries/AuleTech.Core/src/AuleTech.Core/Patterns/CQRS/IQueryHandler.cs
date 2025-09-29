
using AuleTech.Core.Patterns.Result;

namespace AuleTech.Core.Patterns.CQRS;

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}

public interface IListQueryHandler<in TQuery, TItem>
{
    Task<Result<List<TItem>>> HandleGroupAsync(TQuery query, CancellationToken cancellationToken);
}



