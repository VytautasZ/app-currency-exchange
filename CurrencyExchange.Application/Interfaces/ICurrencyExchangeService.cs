using CurrencyExchange.Shared.CurrencyResult;

namespace CurrencyExchange.Application.Interfaces;

public interface ICurrencyExchangeService
{
    Task<ExchangeResult<decimal>> ExchangeCurrencyAsync(string fromCurrency, string toCurrency, decimal amount, CancellationToken cancellationToken);
}
