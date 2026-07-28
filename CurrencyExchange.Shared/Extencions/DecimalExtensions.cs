namespace CurrencyExchange.Shared.Extencions;

public static class DecimalExtensions
{
    public static decimal RoundToGivenDecimalPlaces(this decimal value, int decimalplaces)
    {
        return Math.Round(value, decimalplaces, MidpointRounding.AwayFromZero);
    }
}
