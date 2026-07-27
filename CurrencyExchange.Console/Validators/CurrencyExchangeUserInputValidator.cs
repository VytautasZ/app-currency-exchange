using System.Globalization;
using CurrencyExchange.ConsoleUI.Helpers;

namespace CurrencyExchange.ConsoleUI;

internal static partial class CurrencyExchangeUserInputValidator
{
    public static bool Validate(string? exchangeQueryInput)
    {
        if (string.IsNullOrWhiteSpace(exchangeQueryInput))
        {
            return false;
        }

        var match = CurrencyExchangeQueryRegex.ExchangeQuery().Match(exchangeQueryInput); ;

        if (!match.Success)
        {
            return false;
        }

        if (!decimal.TryParse(
                match.Groups["amount"].Value,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var amount))
        {
            return false;
        }

        return true;
    }
}
