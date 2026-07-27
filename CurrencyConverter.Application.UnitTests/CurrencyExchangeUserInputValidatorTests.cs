using CurrencyExchange.ConsoleUI;

namespace CurrencyConverter.Application.UnitTests;

public class CurrencyExchangeUserInputValidatorTests
{
    [Theory]
    [InlineData("Exchange USD/EUR 100")]
    [InlineData("Exchange EUR/LTU 10")]
    [InlineData("Exchange USD/EUR 100.50")]
    [InlineData("Exchange USD/DKK 0.059740")]
    public void Validate_IfQueryIsWellFormed_ReturnsTrue(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeUserInputValidator.Validate(exchangeQueryInput);

        //Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("exchange usd/eur 100")]
    [InlineData("EXCHANGE USD/EUR 100")]
    [InlineData("ExChAnGe UsD/eUr 100")]
    public void Validate_IfQueryUsesAnyCasing_ReturnsTrue(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeUserInputValidator.Validate(exchangeQueryInput);

        //Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("  Exchange USD/EUR 100  ")]
    [InlineData("Exchange   USD/EUR   100")]
    [InlineData("Exchange USD / EUR 100")]
    public void Validate_IfQueryHasExtraWhitespace_ReturnsTrue(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeUserInputValidator.Validate(exchangeQueryInput);

        //Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_IfInputIsEmpty_ReturnsFalse(string? exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeUserInputValidator.Validate(exchangeQueryInput);

        //Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("Convert USD/EUR 100")]
    [InlineData("USD/EUR 100")]
    [InlineData("Exchanged USD/EUR 100")]
    public void Validate_IfCommandIsWrong_ReturnsFalse(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeUserInputValidator.Validate(exchangeQueryInput);

        //Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("Exchange USDD/EUR 100")]
    [InlineData("Exchange US/EUR 100")]
    [InlineData("Exchange USD/EURO 100")]
    [InlineData("Exchange US1/EUR 100")]
    [InlineData("Exchange USD-EUR 100")]
    [InlineData("Exchange USD EUR 100")]
    [InlineData("Exchange USD/ 100")]
    public void Validate_IfCurrencyPairIsMalformed_ReturnsFalse(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeUserInputValidator.Validate(exchangeQueryInput);

        //Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("Exchange USD/EUR")]
    [InlineData("Exchange USD/EUR abc")]
    [InlineData("Exchange USD/EUR 100,50")]
    [InlineData("Exchange USD/EUR 1.1234567")]
    [InlineData("Exchange USD/EUR 9999999999999999999")]
    [InlineData("Exchange USD/EUR 100 200")]
    public void Validate_IfAmountIsMalformed_ReturnsFalse(string exchangeQueryInput)
    {
        //Arange

        //Act
        var result = CurrencyExchangeUserInputValidator.Validate(exchangeQueryInput);

        //Assert
        Assert.False(result);
    }
}
