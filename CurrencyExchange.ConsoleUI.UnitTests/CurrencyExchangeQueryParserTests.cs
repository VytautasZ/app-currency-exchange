using CurrencyExchange.ConsoleUI.Helpers;

namespace CurrencyExchange.ConsoleUI.UnitTests;

public class CurrencyExchangeQueryParserTests
{
    [Theory]
    [InlineData("Exchange USD/EUR 100", "USD", "EUR", 100)]
    [InlineData("Exchange EUR/LTU 10", "EUR", "LTU", 10)]
    [InlineData("Exchange USD/EUR 100.50", "USD", "EUR", 100.50)]
    [InlineData("Exchange USD/DKK 0.059740", "USD", "DKK", 0.059740)]
    public void ParseQueryString_IfQueryIsWellFormed_ReturnsQuery(
        string exchangeQueryInput,
        string expectedFromCurrency,
        string expectedToCurrency,
        decimal expectedAmount)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.True(result.Success);
        Assert.Equal(expectedFromCurrency, result.Value.FromCurrency);
        Assert.Equal(expectedToCurrency, result.Value.ToCurrency);
        Assert.Equal(expectedAmount, result.Value.Amount);
    }

    [Theory]
    [InlineData("exchange usd/eur 100")]
    [InlineData("EXCHANGE USD/EUR 100")]
    [InlineData("ExChAnGe UsD/eUr 100")]
    public void ParseQueryString_IfQueryUsesAnyCasing_ReturnsUppercasedCurrencies(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.True(result.Success);
        Assert.Equal("USD", result.Value.FromCurrency);
        Assert.Equal("EUR", result.Value.ToCurrency);
    }

    [Theory]
    [InlineData("  Exchange USD/EUR 100  ")]
    [InlineData("Exchange   USD/EUR   100")]
    [InlineData("Exchange USD / EUR 100")]
    public void ParseQueryString_IfQueryHasExtraWhitespace_ReturnsQuery(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.True(result.Success);
        Assert.Equal("USD", result.Value.FromCurrency);
        Assert.Equal("EUR", result.Value.ToCurrency);
        Assert.Equal(100, result.Value.Amount);
    }

    [Fact]
    public void ParseQueryString_IfCurrenciesAreSame_ReturnsQuery()
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString("Exchange USD/USD 100");

        //Assert
        Assert.True(result.Success);
        Assert.Equal("USD", result.Value.FromCurrency);
        Assert.Equal("USD", result.Value.ToCurrency);
    }

    [Theory]
    [InlineData("Exchange USD/EUR 999999999999999999", 999999999999999999)]
    [InlineData("Exchange USD/EUR 0.000001", 0.000001)]
    public void ParseQueryString_IfAmountIsAtTheAllowedLimit_ReturnsQuery(string exchangeQueryInput, decimal expectedAmount)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.True(result.Success);
        Assert.Equal(expectedAmount, result.Value.Amount);
    }

    [Theory]
    [InlineData("Exchange USD/EUR -5", -5)]
    [InlineData("Exchange USD/EUR 0", 0)]
    public void ParseQueryString_IfAmountIsNotPositive_ReturnsQueryForTheServiceToReject(string exchangeQueryInput, decimal expectedAmount)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedAmount, result.Value.Amount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseQueryString_IfInputIsEmpty_ReturnsFailure(string? exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal("Invalid exchange query format.", result.Error);
    }

    [Theory]
    [InlineData("Convert USD/EUR 100")]
    [InlineData("USD/EUR 100")]
    [InlineData("Exchanged USD/EUR 100")]
    public void ParseQueryString_IfCommandIsWrong_ReturnsFailure(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal("Invalid exchange query format.", result.Error);
    }

    [Theory]
    [InlineData("Exchange USDD/EUR 100")]
    [InlineData("Exchange US/EUR 100")]
    [InlineData("Exchange USD/EURO 100")]
    [InlineData("Exchange US1/EUR 100")]
    [InlineData("Exchange USD-EUR 100")]
    [InlineData("Exchange USD EUR 100")]
    [InlineData("Exchange USD/ 100")]
    [InlineData("Exchange /EUR 100")]
    public void ParseQueryString_IfCurrencyPairIsMalformed_ReturnsFailure(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal("Invalid exchange query format.", result.Error);
    }

    [Theory]
    [InlineData("Exchange USD/EUR")]
    [InlineData("Exchange USD/EUR abc")]
    [InlineData("Exchange USD/EUR 100,50")]
    [InlineData("Exchange USD/EUR 1.1234567")]
    [InlineData("Exchange USD/EUR 9999999999999999999")]
    [InlineData("Exchange USD/EUR 100 200")]
    [InlineData("Exchange USD/EUR 100.")]
    [InlineData("Exchange USD/EUR .50")]
    public void ParseQueryString_IfAmountIsMalformed_ReturnsFailure(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryInput);

        //Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal("Invalid exchange query format.", result.Error);
    }

    [Fact]
    public void ParseQueryString_IfQueryIsInvalid_ReturnsErrorMessageAndNoValue()
    {
        //Arange

        //Act
        var result = CurrencyExchangeQueryParser.ParseQueryString("not a query");

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid exchange query format.", result.Error);
        Assert.Null(result.Value);
    }
}
