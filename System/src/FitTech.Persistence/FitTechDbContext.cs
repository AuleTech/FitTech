using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Aggregates.EmailAggregate;
using FitTech.Domain.Aggregates.TrainerAggregate;
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
    public DbSet<Trainer> Trainer { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Email>().HasKey(x => x.Id);

        base.OnModelCreating(builder);
    }
}
