namespace CurrencyConverter.Application.UnitTests;

public class CurrencyExchangeTests
{
    [Fact]
    public void ExchangeCurrency_IfCurrencyUnknown_ReturnsErrorMessage()
    {
        //Arange

        //Act
        var result = CurrrencyExchangeService.ExchangeCurrency("USD", "ABC", 100);

        //Assert
        Assert.Equal("Unknown currency", result);
    }

    [Fact]
    public void ExchangeCurrency_IfCurrenciesAreSame_ReturnsSameAmount()
    {
        //Arange

        //Act
        var result = CurrrencyExchangeService.ExchangeCurrency("USD", "USD", 591);

        //Assert
        Assert.Equal("591", result);
    }

    private static readonly IReadOnlyDictionary<string, decimal> CurrencyRates =
        new Dictionary<string, decimal>
        {
            ["EUR/DKK"] = 7.4394m,    
            ["USD/DKK"] = 6.6311m,    
            ["GBP/DKK"] = 8.5285m,    
            ["SEK/DKK"] = 0.7610m,    
            ["NOK/DKK"] = 0.7840m,   
            ["CHF/DKK"] = 6.8358m,    
            ["JPY/DKK"] = 0.059740m,  
            ["DKK/DKK"] = 1.0m,    
        };

    private static class CurrrencyExchangeService
    {
        internal static string ExchangeCurrency(string fromCurrency, string toCurrency, decimal amount)
        {

            if (fromCurrency == toCurrency)
            {
                return amount.ToString();
            }

            CurrencyRates.TryGetValue($"{fromCurrency}/{toCurrency}", out var rate);
            if (rate == 0)
            {
                return "Unknown currency";
            }

            return (amount * rate).ToString("F2");
        }
    }
}
