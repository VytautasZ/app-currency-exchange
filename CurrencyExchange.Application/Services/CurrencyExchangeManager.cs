using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Domain.Models;
using CurrencyExchange.Shared.CurrencyResult;

namespace CurrencyExchange.Application.Services;

public class CurrencyExchangeManager : ICurrencyExchangeManager
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ICurrencyExchangeService _currencyExchangeService;

    public CurrencyExchangeManager(ICurrencyExchangeService currencyExchangeService, ICurrencyRepository currencyRepository)
    {
        _currencyExchangeService = currencyExchangeService;
        _currencyRepository = currencyRepository;
    }

    public async Task<ExchangeResult<decimal>> ProceedCurrencyExchangeAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken)
    {
        if (!await CurrenciesExistsAsync(currencyExchangeQuery, cancellationToken))
        {
            return ExchangeResult<decimal>.Fail("Unknown currency");
        }
                
        return await _currencyExchangeService.ExchangeCurrencyAsync(currencyExchangeQuery, cancellationToken);
    }

    private async Task<bool> CurrenciesExistsAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken)
    {
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency }.Distinct().ToList();
        var existingCurenciesCount = await _currencyRepository.GetCountOfExistingCurrienciesAsync(currencies, cancellationToken: cancellationToken);
        return currencies.Count == existingCurenciesCount;
    }
}
