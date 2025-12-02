using FitTech.Domain.Aggregates.ClientAggregate;
using FitTech.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitTech.Persistence.Configuration;

public sealed class ClientSettingsEntityConfiguration : IEntityTypeConfiguration<ClientSettings>
{
    public void Configure(EntityTypeBuilder<ClientSettings> builder)
    {
        builder.Property(x => x.Goal)
            .HasColumnType("varchar")
            .HasConversion(x => x.ToString(), x => Enum.Parse<TrainingGoal>(x, true));
    }
}
