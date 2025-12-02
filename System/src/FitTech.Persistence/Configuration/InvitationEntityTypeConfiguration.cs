using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitTech.Persistence.Configuration;

public sealed class InvitationEntityTypeConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.Property(x => x.Status)
            .HasColumnType("varchar")
            .HasConversion(x => x.ToString(), x => Enum.Parse<InvitationStatus>(x, true));
    }
}
