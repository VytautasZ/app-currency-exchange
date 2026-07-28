using CurrencyExchange.Domain.Models;
using CurrencyExchange.Infrastructure.Persistence;
using CurrencyExchange.Interfaces.Application;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.Repositories;

public class CurrencyExchangeRateRepository : ICurrencyExchangeRateRepository
{
    private readonly CurrencyExchangeDbContext _dbContext;

    public CurrencyExchangeRateRepository(CurrencyExchangeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CurrencyRate>> GetAllCurrencyExchangeRatesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.CurrencyRates
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<CurrencyRate?> GetCurrencyExchangeRateAsync(string mainCurrency, string moneyCurrency, CancellationToken cancellationToken)
    {
        return await _dbContext.CurrencyRates
            .AsNoTracking()
            .FirstOrDefaultAsync(
                rate => rate.MainCurrency == mainCurrency && rate.MoneyCurrency == moneyCurrency,
                cancellationToken);
    }
}
