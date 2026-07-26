using CurrencyExchange.Domain.Models;
using CurrencyExchange.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyExchange.Infrastructure.Persistence.Configurations;

internal sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies");

        builder.HasKey(currency => currency.Id);

        builder.Property(currency => currency.Id)
            .ValueGeneratedOnAdd();

        builder.Property(currency => currency.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(currency => currency.DisplayName)
            .HasMaxLength(100);

        builder.HasData(SeedData.Currencies);
    }
}
