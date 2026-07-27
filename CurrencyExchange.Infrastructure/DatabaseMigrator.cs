using CurrencyExchange.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyExchange.Infrastructure;

public static class DatabaseMigrator
{
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyExchangeDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
