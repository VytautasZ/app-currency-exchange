using System.Text.RegularExpressions;

namespace CurrencyExchange.ConsoleUI.Helpers;

public static partial class CurrencyExchangeQueryRegex
{
    [GeneratedRegex(
        @"^\s*Exchange\s+(?<from>[A-Za-z]{3})\s*/\s*(?<to>[A-Za-z]{3})\s+(?<amount>-?\d{1,18}(\.\d{1,6})?)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    public static partial Regex ExchangeQuery();
}
