using System.Net.NetworkInformation;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Aggregates.ClientAggregate;
using FitTech.Domain.Aggregates.EmailAggregate;
using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Persistence.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence;

public class FitTechDbContext : IdentityDbContext<FitTechUser, FitTechRole, Guid>
{
    public FitTechDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Email> Emails { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<ClientSettings> ClientSettings { get; set; }
    public DbSet<BodyMeasurement> BodyMeasurements { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ClientEntityTypeConfiguration());
        
        base.OnModelCreating(builder);
    }
}
