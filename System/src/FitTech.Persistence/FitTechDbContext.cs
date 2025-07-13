using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence;

public class FitTechDbContext : IdentityDbContext<FitTechUser, FitTechRole, Guid>
{
    public FitTechDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Email> EmailLog { get; set; }
    public DbSet<Client> Client { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Email>().HasKey(x => x.Id);

        base.OnModelCreating(builder);
    }
}
