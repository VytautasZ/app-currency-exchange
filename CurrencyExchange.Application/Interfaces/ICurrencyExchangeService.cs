using CurrencyExchange.Domain.Models;
using CurrencyExchange.Shared.CurrencyResult;

namespace CurrencyExchange.Application.Interfaces;

public interface ICurrencyExchangeService
{
    Task<ExchangeResult<decimal>> ExchangeCurrencyAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken);
}
