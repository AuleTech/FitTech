using FitTech.Domain.Aggregates.ClientAggregate;
using FitTech.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitTech.Persistence.Configuration;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.OwnsOne<Address>(x => x.Address);
    }
}
