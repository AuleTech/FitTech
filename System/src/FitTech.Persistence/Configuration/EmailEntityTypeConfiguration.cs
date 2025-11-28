using FitTech.Domain.Aggregates.EmailAggregate;
using FitTech.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitTech.Persistence.Configuration;

public sealed class EmailEntityTypeConfiguration : IEntityTypeConfiguration<Email>
{
    public void Configure(EntityTypeBuilder<Email> builder)
    {
        builder.Property<EmailType>(x => x.EmailType)
            .HasColumnType("nvarchar")
            .HasConversion(x => x.ToString(), x => Enum.Parse<EmailType>(x, true));
    }
}
