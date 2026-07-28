using System.Globalization;
using System.Text.RegularExpressions;
using CurrencyExchange.Domain.Models;
using CurrencyExchange.Shared.CurrencyResult;

namespace CurrencyExchange.ConsoleUI.Helpers;

public static class CurrencyExchangeQueryParser
{
    public static ExchangeResult<CurrencyExchangeQuery> ParseQueryString(string? exchangeQueryInput)
    {
        if (string.IsNullOrWhiteSpace(exchangeQueryInput))
        {
            return ExchangeResult<CurrencyExchangeQuery>.Fail("Invalid exchange query format.");
        }

        var match = CurrencyExchangeQueryRegex.ExchangeQuery().Match(exchangeQueryInput); ;

        if (!match.Success)
        {
            return ExchangeResult<CurrencyExchangeQuery>.Fail("Invalid exchange query format.");
        }

        if (!decimal.TryParse(
                match.Groups["amount"].Value,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var amount))
        {
            return ExchangeResult<CurrencyExchangeQuery>.Fail("Invalid exchange query format.");
        }

        return ExchangeResult<CurrencyExchangeQuery>.Ok(exchangeQueryInput.ToCurrencyExchangeQuery(amount, match));
    }

    private static CurrencyExchangeQuery ToCurrencyExchangeQuery(this string exchangeQueryInput, decimal amount, Match match)
    {
        return new CurrencyExchangeQuery
        {
            FromCurrency = match.Groups["from"].Value.ToUpperInvariant(),
            ToCurrency = match.Groups["to"].Value.ToUpperInvariant(),
            Amount = amount
        };
    }
}
