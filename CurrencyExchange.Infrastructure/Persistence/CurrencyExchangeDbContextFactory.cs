using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CurrencyExchange.Infrastructure.Persistence;

public class CurrencyExchangeDbContextFactory : IDesignTimeDbContextFactory<CurrencyExchangeDbContext>
{
    private const string ConnectionStringVariable = "CURRENCYEXCHANGE_CONNECTIONSTRING";

    private const string DefaultConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=CurrencyExchange;Trusted_Connection=True;TrustServerCertificate=True";

    public CurrencyExchangeDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(ConnectionStringVariable) ?? DefaultConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder<CurrencyExchangeDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new CurrencyExchangeDbContext(optionsBuilder.Options);
    }
}
