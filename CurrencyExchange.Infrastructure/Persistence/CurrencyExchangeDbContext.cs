using CurrencyExchange.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.Persistence;

public class CurrencyExchangeDbContext : DbContext
{
    public CurrencyExchangeDbContext(DbContextOptions<CurrencyExchangeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<CurrencyRate> CurrencyRates => Set<CurrencyRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CurrencyExchangeDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
