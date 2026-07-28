using CurrencyExchange.Application.Services;
using CurrencyExchange.Domain.Models;

namespace CurrencyConverter.Application.UnitTests;

public class CrossCurrencyRateResolverTests
{
    [Fact]
    public void ResolveExchangeRate_IfDirectRateExists_ReturnsRate()
    {
        //Arange
        var currencyRates = new[] { CreateRate("EUR", "DKK", 2m) };

        //Act
        var result = CrossCurrencyRateResolver.ResolveExchangeRateAsync("EUR", "DKK", currencyRates, cancellationToken: default);

        //Assert
        Assert.Equal(2m, result);
    }

    [Fact]
    public void ResolveExchangeRate_IfOnlyInverseRateExists_ReturnsInvertedRate()
    {
        //Arange
        var currencyRates = new[] { CreateRate("EUR", "DKK", 2m) };

        //Act
        var result = CrossCurrencyRateResolver.ResolveExchangeRateAsync("DKK", "EUR", currencyRates, cancellationToken: default);

        //Assert
        Assert.Equal(0.5m, result);
    }

    [Fact]
    public void ResolveExchangeRate_IfNoDirectRateExists_ResolvesThroughMoneyCurrency()
    {
        //Arange
        var currencyRates = new[]
        {
            CreateRate("EUR", "DKK", 8m),
            CreateRate("USD", "DKK", 4m)
        };

        //Act
        var result = CrossCurrencyRateResolver.ResolveExchangeRateAsync("EUR", "USD", currencyRates, cancellationToken: default);

        //Assert
        Assert.Equal(2m, result);
    }

    [Fact]
    public void ResolveExchangeRate_IfRouteNeedsThreeSteps_ReturnsCorrectRate()
    {
        //Arange
        var currencyRates = new[]
        {
            CreateRate("EUR", "DKK", 2m),
            CreateRate("DKK", "JPY", 3m),
            CreateRate("JPY", "USD", 5m)
        };

        //Act
        var result = CrossCurrencyRateResolver.ResolveExchangeRateAsync("EUR", "USD", currencyRates, cancellationToken: default);

        //Assert
        Assert.Equal(30m, result);
    }

    [Fact]
    public void ResolveExchangeRate_WithSeededRates_ReturnsExpectedCrossRate()
    {
        //Arange
        var currencyRates = new[]
        {
            CreateRate("EUR", "DKK", 7.4394m),
            CreateRate("USD", "DKK", 6.6311m)
        };

        //Act
        var result = CrossCurrencyRateResolver.ResolveExchangeRateAsync("EUR", "USD", currencyRates, cancellationToken: default);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(1.121895m, Math.Round(result.Value, 6));
    }

    [Fact]
    public void ResolveExchangeRate_IfDirectRateExists_PrefersItOverLongerRoute()
    {
        //Arange
        var currencyRates = new[]
        {
            CreateRate("EUR", "USD", 10m),
            CreateRate("EUR", "DKK", 2m),
            CreateRate("DKK", "USD", 1m)
        };

        //Act
        var result = CrossCurrencyRateResolver.ResolveExchangeRateAsync("EUR", "USD", currencyRates, cancellationToken: default);

        //Assert
        Assert.Equal(10m, result);
    }

    [Fact]
    public void ResolveExchangeRate_IfCurrenciesAreNotConnected_ReturnsNull()
    {
        //Arange
        var currencyRates = new[]
        {
            CreateRate("EUR", "DKK", 2m),
            CreateRate("USD", "JPY", 3m)
        };

        //Act
        var result = CrossCurrencyRateResolver.ResolveExchangeRateAsync("EUR", "USD", currencyRates, cancellationToken: default);

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public void ResolveExchangeRate_IfToCurrencyHasNoRates_ReturnsNull()
    {
        //Arange
        var currencyRates = new[] { CreateRate("EUR", "DKK", 2m) };

        //Act
        var result = CrossCurrencyRateResolver.ResolveExchangeRateAsync("EUR", "SEK", currencyRates, cancellationToken: default);

        //Assert
        Assert.Null(result);
    }

    private static CurrencyRate CreateRate(string mainCurrency, string moneyCurrency, decimal rate)
    {
        return new CurrencyRate
        {
            MainCurrency = mainCurrency,
            MoneyCurrency = moneyCurrency,
            Rate = rate
        };
    }
}
