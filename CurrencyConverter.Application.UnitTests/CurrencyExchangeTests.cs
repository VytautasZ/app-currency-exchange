using CurrencyExchange.Domain.Models;

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

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void ExchangeCurrency_IfAmountIsNegativeOrZero_ReturnsError(decimal amount)
    {
        //Arange

        //Act
        var result = CurrrencyExchangeService.ExchangeCurrency("USD", "DKK", amount);

        //Assert
        Assert.Equal("Invalid amount", result);
    }

    [Theory]
    [InlineData("EUR", "DKK", 100, "743.94")]
    [InlineData("JPY", "DKK", 100, "5.97")]
    [InlineData("SEK", "DKK", 100, "76.10")]
    public void ExchangeCurrency_IfCurrencyRateIsInList_ReturnsCorrectResult(string currencyFrom, string currencyTo, decimal amount, string expectedResult)
    {
        //Arange

        //Act
        var result = CurrrencyExchangeService.ExchangeCurrency(currencyFrom, currencyTo, amount);

        //Assert
        Assert.Equal(expectedResult, result);
    }

    private static readonly IReadOnlyList<CurrencyRate> CurrencyRates =
        new List<CurrencyRate>
        {
            new CurrencyRate { MainCurrency = "EUR", MoneyCurrency = "DKK", Rate = 7.4394m },
            new CurrencyRate { MainCurrency = "USD", MoneyCurrency = "DKK", Rate = 6.6311m },
            new CurrencyRate { MainCurrency = "GBP", MoneyCurrency = "DKK", Rate = 8.5285m },
            new CurrencyRate { MainCurrency = "SEK", MoneyCurrency = "DKK", Rate = 0.7610m },
            new CurrencyRate { MainCurrency = "NOK", MoneyCurrency = "DKK", Rate = 0.7840m },
            new CurrencyRate { MainCurrency = "CHF", MoneyCurrency = "DKK", Rate = 6.8358m },
            new CurrencyRate { MainCurrency = "JPY", MoneyCurrency = "DKK", Rate = 0.059740m },
        };

    private static class CurrrencyExchangeService
    {
        internal static string ExchangeCurrency(string fromCurrency, string toCurrency, decimal amount)
        {
            if (amount <= 0)
            {
                return "Invalid amount";
            }

            if (fromCurrency == toCurrency)
            {
                return amount.ToString();
            }

            var rate = CurrencyRates.FirstOrDefault(cr => cr.MainCurrency == fromCurrency && cr.MoneyCurrency == toCurrency);
            if (rate == null)
            {
                return "Unknown currency";
            }

            return (amount * rate.Rate).ToString("F2");
        }
    }
}
