using CurrencyExchange.Domain.Models;

namespace CurrencyExchange.Interfaces.Application;

public interface ICurrencyExchangeRateRepository
{
    Task<IEnumerable<CurrencyRate>> GetAllCurrencyExchangeRatesAsync(CancellationToken cancellationToken);
    Task<CurrencyRate?> GetCurrencyExchangeRateAsync(string mainCurrency, string moneyCurrency, CancellationToken cancellationToken);
}
