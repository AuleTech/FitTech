using FitTech.Domain.Entities;

namespace FitTech.Domain.Repositories;

public interface ITrainerRepository
{
    Task<Trainer?> GetByIdAsync(Guid trainerId, CancellationToken cancellationToken);
}
