using FitTech.Domain.Aggregates.TrainerAggregate;

namespace FitTech.Domain.Repositories;

public interface ITrainerRepository
{
    Task<Trainer?> GetAsync(Guid Id, CancellationToken cancellationToken);
    Task<Trainer> AddAsync(Trainer trainer, CancellationToken cancellationToken);
    Task<Invitation> AddInvitationAsync(Invitation invitation, CancellationToken cancellationToken);
}
