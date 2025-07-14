using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence.Repositories;

public class TrainerRepository: ITrainerRepository
{
    private readonly FitTechDbContext _context;

    public TrainerRepository(FitTechDbContext context)
    {
        _context = context;
    }

    public async Task<Trainer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Trainer.FindAsync([id], cancellationToken);
    }

    public async Task<Trainer?> UpdateTrainerAsync(Guid id, string name, string email, string password, CancellationToken cancellationToken)
    {
        var trainer = await _context.Trainer.FindAsync([id], cancellationToken);

        if (trainer is null)
        {
            return null;   
        }

        trainer.UpdateData(name, email,password, cancellationToken); 

        await _context.SaveChangesAsync(cancellationToken);

        return trainer;
    }
}
