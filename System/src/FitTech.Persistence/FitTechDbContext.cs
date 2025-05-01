using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence;

public class FitTechDbContext : IdentityDbContext<FitTechUser, FitTechRole, Guid>
{
    public DbSet<ResetPasswordEmail> ResetPasswordEmail { get; set; }
    public FitTechDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        
    } 
}
