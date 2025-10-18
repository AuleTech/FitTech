using FitTech.Domain.Aggregates.TrainerAggregate;

namespace FitTech.Domain.Repositories;

public interface ITrainerRepository
{
    Task<Trainer> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Trainer> GetByInvitationId(Guid invitationId, CancellationToken cancellationToken);
    Task<Trainer> AddAsync(Trainer trainer, CancellationToken cancellationToken);
    Task<Invitation> AddInvitationAsync(Invitation invitation, CancellationToken cancellationToken);
}
