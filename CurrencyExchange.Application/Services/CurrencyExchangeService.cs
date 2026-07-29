using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Domain.Models;
using CurrencyExchange.Interfaces.Application;
using CurrencyExchange.Shared.CurrencyResult;
using CurrencyExchange.Shared.Extencions;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyExchange.Application.Services;

public class CurrencyExchangeService : ICurrencyExchangeService
{
    private readonly ICurrencyExchangeRateRepository _currencyRateRepository;
    private readonly IMemoryCache memoryCache;

    public CurrencyExchangeService(ICurrencyExchangeRateRepository currencyRateRepository, IMemoryCache memoryCache)
    {
        _currencyRateRepository = currencyRateRepository;
        this.memoryCache = memoryCache;
    }

    public async Task<ExchangeResult<decimal>> ExchangeCurrencyAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken)
    {
        var calculatedRateResult = await ResolveCrossCurrencyExchangeRateAsync(currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency, cancellationToken);
        if (calculatedRateResult != null)
        {
            return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(calculatedRateResult.Value, currencyExchangeQuery.Amount));
        }

        return ExchangeResult<decimal>.Fail("No rate was calculated");
    }

    private static decimal CalculateRoundedExchangedAmount(decimal rate, decimal amount)
    {
        var result = amount * rate;
        return result.RoundToGivenDecimalPlaces(2);
    }

    private async Task<decimal?> ResolveCrossCurrencyExchangeRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken)
    {
        var currencyRates = await GetAllCurrencyRates(cancellationToken);
        if (currencyRates == null || !currencyRates.Any())
        {
            return null;
        }

        return CrossCurrencyRateResolver.ResolveExchangeRateAsync(fromCurrency, toCurrency, currencyRates, cancellationToken);
    }

    private async Task<IEnumerable<CurrencyRate>> GetAllCurrencyRates(CancellationToken cancellationToken)
    {
        string key = "allcurrencyrates";
        if (!memoryCache.TryGetValue(key, out IEnumerable<CurrencyRate>? currencyRates))
        {
            currencyRates = await _currencyRateRepository.GetAllCurrencyExchangeRatesAsync(cancellationToken);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(10))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

            memoryCache.Set(key, currencyRates, cacheOptions);
        }
        return currencyRates ?? Array.Empty<CurrencyRate>();
    }
}
