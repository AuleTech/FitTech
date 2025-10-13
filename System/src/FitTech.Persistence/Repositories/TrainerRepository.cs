using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence.Repositories;

internal class TrainerRepository : ITrainerRepository
{
    private readonly FitTechDbContext _context;

    public TrainerRepository(FitTechDbContext context)
    {
        _context = context;
    }

    public async Task<Trainer> AddAsync(Trainer trainer, CancellationToken cancellationToken)
    {
        await _context.Trainer.AddAsync(trainer, cancellationToken);

        return trainer;
    }

    public async Task<Trainer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Trainer.FindAsync([id], cancellationToken);
    }

    public async Task<Trainer?> GetAsync(Guid trainerId, CancellationToken cancellationToken)
    {
        return await _context.Trainer.SingleAsync(x => x.Id == trainerId, cancellationToken);
    }
}
