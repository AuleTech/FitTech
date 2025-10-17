using AuleTech.Core.Patterns.CQRS;
using FitTech.Domain.Seedwork;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands;

public abstract class TransactionCommandHandler<TCommand, TResponse> : IAuleTechCommandHandler<TCommand, TResponse> where TCommand : ICommand
{
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly ILogger<TransactionCommandHandler<TCommand, TResponse>> Logger;
    protected TransactionCommandHandler(IUnitOfWork unitOfWork, ILogger<TransactionCommandHandler<TCommand, TResponse>> logger)
    {
        UnitOfWork = unitOfWork;
        Logger = logger;
    }

    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await UnitOfWork.OpenTransactionAsync(cancellationToken);
        try
        {
            Logger.LogTrace("Starting Transaction for {CommandName}", command.GetType().Name);
            var response = await HandleTransactionAsync(command, cancellationToken);

            Logger.LogTrace("Commiting Transaction");
            await UnitOfWork.SaveAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch (Exception)
        {
            Logger.LogError("Error during transaction, rolling back...");
            await transaction.RollbackAsync(cancellationToken);
            
            throw;
        }
    }

    protected abstract Task<TResponse> HandleTransactionAsync(TCommand command, CancellationToken cancellationToken);
}
