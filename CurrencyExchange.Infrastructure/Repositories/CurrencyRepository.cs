using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly CurrencyExchangeDbContext _dbContext;

    public CurrencyRepository(CurrencyExchangeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CurrencyExistsAsync(IEnumerable<string> currencyCodes, CancellationToken cancellationToken)
    {
        return await _dbContext.Currencies
            .AsNoTracking()
            .AnyAsync(currency => currencyCodes.Contains(currency.CurrencyCode), cancellationToken);
    }
}
