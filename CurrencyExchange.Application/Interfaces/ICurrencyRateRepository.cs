using CurrencyExchange.Domain.Models;

namespace CurrencyExchange.Interfaces.Application;

public interface ICurrencyRateRepository
{
    Task<bool> CurrencyExistsAsync(string currency, CancellationToken cancellationToken);
    Task<IEnumerable<CurrencyRate>> GetAllCurrencyRatesAsync(CancellationToken cancellationToken);
    Task<CurrencyRate?> GetCurrencyRateAsync(string mainCurrency, string moneyCurrency, CancellationToken cancellationToken);
}
