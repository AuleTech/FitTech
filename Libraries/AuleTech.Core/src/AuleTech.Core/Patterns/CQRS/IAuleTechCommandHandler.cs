namespace AuleTech.Core.Patterns.CQRS;

public interface IAuleTechCommandHandler<in TCommand, TResult> where TCommand : ICommand
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
