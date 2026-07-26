using CurrencyExchange.Domain.Models;
using CurrencyExchange.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyExchange.Infrastructure.Persistence.Configurations;

internal sealed class CurrencyRateConfiguration : IEntityTypeConfiguration<CurrencyRate>
{
    public void Configure(EntityTypeBuilder<CurrencyRate> builder)
    {
        builder.ToTable("CurrencyRates");

        builder.HasKey(rate => rate.Id);
        
        builder.Property(rate => rate.Id)
            .ValueGeneratedOnAdd();

        builder.Property(rate => rate.MainCurrency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(rate => rate.MoneyCurrency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(rate => rate.Rate)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(rate => rate.MainCurrency)
            .HasPrincipalKey(currency => currency.CurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(rate => rate.MoneyCurrency)
            .HasPrincipalKey(currency => currency.CurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rate => rate.MainCurrency);
        builder.HasIndex(rate => rate.MoneyCurrency);

        builder.ToTable(table => table.HasCheckConstraint("CK_CurrencyRates_Rate_Positive", "[Rate] > 0"));

        builder.HasData(SeedData.CurrencyRates);
    }
}
