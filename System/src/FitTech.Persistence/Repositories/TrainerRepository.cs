using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence.Repositories;

public class TrainerRepository: ITrainerRepository
{
    private readonly FitTechDbContext _context;

    public TrainerRepository(FitTechDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Trainer>> AddAsync(Trainer trainer, CancellationToken cancellationToken)
    {
      
        await _context.Trainer.AddAsync(trainer, cancellationToken);
        var rows = await _context.SaveChangesAsync(cancellationToken);
        
        var hasher = new PasswordHasher<Trainer>();
        var hashedPassword = hasher.HashPassword(trainer, trainer.Password);

        var UserIdentity = new FitTechUser
        {
            Id = trainer.Id, Email = trainer.Email, UserName = trainer.Name, PasswordHash = hashedPassword,

        };
        
        await _context.Users.AddAsync(UserIdentity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
            
        return rows < 1 ? Result<Trainer>.Failure("Nothing was saved") : Result<Trainer>.Success(trainer);
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

        trainer.UpdateData(id,name, email,password, cancellationToken); 

        await _context.SaveChangesAsync(cancellationToken);

        return trainer;
    }
    
    public async Task<Trainer?> GetAsync(Guid trainerId, CancellationToken cancellationToken)
    {
        return await _context.Trainer.SingleAsync(x => x.Id == trainerId, cancellationToken);
    }
}
