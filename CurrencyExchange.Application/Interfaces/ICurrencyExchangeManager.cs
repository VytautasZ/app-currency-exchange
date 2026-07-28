using CurrencyExchange.Domain.Models;
using CurrencyExchange.Shared.CurrencyResult;

namespace CurrencyExchange.Application.Interfaces;

public interface ICurrencyExchangeManager
{
    Task<ExchangeResult<decimal>> ProceedCurrencyExchangeAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken);
}
