using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Domain.Models;
using CurrencyExchange.Interfaces.Application;
using CurrencyExchange.Shared.CurrencyResult;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyExchange.Application.Services;

public class CurrencyExchangeService : ICurrencyExchangeService
{
    private readonly ICurrencyExchangeRateRepository _currencyRateRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IMemoryCache memoryCache;

    public CurrencyExchangeService(ICurrencyExchangeRateRepository currencyRateRepository, ICurrencyRepository currencyRepository, IMemoryCache memoryCache)
    {
        _currencyRateRepository = currencyRateRepository;
        _currencyRepository = currencyRepository;
        this.memoryCache = memoryCache;
    }

    public async Task<ExchangeResult<decimal>> ExchangeCurrencyAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken)
    {
        if (currencyExchangeQuery.Amount <= 0)
        {
            return ExchangeResult<decimal>.Fail("Invalid amount");
        }

        if (!await CurrenciesExistsAsync(currencyExchangeQuery, cancellationToken))
        {
            return ExchangeResult<decimal>.Fail("Unknown currency");
        }

        if (currencyExchangeQuery.FromCurrency == currencyExchangeQuery.ToCurrency)
        {
            return ExchangeResult<decimal>.Ok(currencyExchangeQuery.Amount);
        }

        var rate = await _currencyRateRepository.GetCurrencyExchangeRateAsync(currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency, cancellationToken);
        if (rate != null)
        {
            return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(rate.Rate, currencyExchangeQuery.Amount));
        }

        var calculatedRateResult = await ResolveCrossCurrencyExchangeRateAsync(currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency, cancellationToken);
        if(calculatedRateResult != null)
        {
            return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(calculatedRateResult.Value, currencyExchangeQuery.Amount));
        }

        return ExchangeResult<decimal>.Fail("No rate was calculated");
    }

    private async Task<bool> CurrenciesExistsAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken)
    {
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency }.Distinct().ToList();
        var existingCurenciesCount = await _currencyRepository.GetCountOfExistingCurrienciesAsync(currencies, cancellationToken: cancellationToken);
        return currencies.Count == existingCurenciesCount;
    }

    private static decimal CalculateRoundedExchangedAmount(decimal rate, decimal amount)
    {
        var result = amount * rate;
        return RoundToTwoDecimalPlaces(result);
    }

    private static decimal RoundToTwoDecimalPlaces(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
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
