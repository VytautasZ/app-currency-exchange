using System.Globalization;
using CurrencyExchange.ConsoleUI.Helpers;
using CurrencyExchange.Domain.Models;

namespace CurrencyExchange.ConsoleUI.Mapper;

public static class CurrencyExchangeQueryMapper
{
    public static CurrencyExchangeQuery ToCurrencyExchangeQuery(this string exchangeQueryInput)
    {
        var match = CurrencyExchangeQueryRegex.ExchangeQuery().Match(exchangeQueryInput);
        return new CurrencyExchangeQuery
        {
            FromCurrency = match.Groups["from"].Value.ToUpperInvariant(),
            ToCurrency = match.Groups["to"].Value.ToUpperInvariant(),
            Amount = decimal.Parse(match.Groups["amount"].Value, NumberStyles.Number, CultureInfo.InvariantCulture)
        };
    }
}
