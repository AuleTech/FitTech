using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence;

public class FitTechDbContext : IdentityDbContext<FitTechUser, FitTechRole, Guid>
{
    public DbSet<Email> ResetPasswordEmail { get; set; }
    public FitTechDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Email>().HasKey(x => x.Id);
        
        base.OnModelCreating(builder);
    }
}
