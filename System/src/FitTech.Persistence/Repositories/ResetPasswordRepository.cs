using FitTech.Domain.Entities;
using FitTech.Domain.Interfaces;


namespace FitTech.Persistence.Repositories;

public class ResetPasswordRepository : IResetPasswordEmail
{
    private readonly FitTechDbContext _context;

    public ResetPasswordRepository(FitTechDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(ResetPasswordEmail resetPasswordEmail)
    {
        await _context.ResetPasswordEmail.AddAsync(resetPasswordEmail);
        await _context.SaveChangesAsync();
    }
}
