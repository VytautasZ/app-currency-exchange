using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Application.Services;
using CurrencyExchange.Domain.Models;
using CurrencyExchange.Interfaces.Application;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace CurrencyConverter.Application.UnitTests;

public class CurrencyExchangeTests
{
    private readonly ICurrencyExchangeRateRepository _currencyRateRepository = Substitute.For<ICurrencyExchangeRateRepository>();
    private readonly ICurrencyRepository _currencyRepository = Substitute.For<ICurrencyRepository>();
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly CurrencyExchangeService _sut;

    public CurrencyExchangeTests()
    {
        _sut = new CurrencyExchangeService(_currencyRateRepository, _currencyRepository, _memoryCache);
    }

    [Fact]
    public async Task ExchangeCurrency_IfCurrencyUnknown_ReturnsErrorMessage()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "USD", ToCurrency = "ABC" };
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency }.AsEnumerable();
        
        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
           .Returns(1);

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyExchangeQuery, cancellationToken:default);

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Unknown currency", result.Error);
    }

    [Fact]
    public async Task ExchangeCurrency_IfBothCurrenciesUnknown_ReturnsErrorMessage()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "ABC", ToCurrency = "ABC" };
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency }.AsEnumerable();

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
           .Returns(0);

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Unknown currency", result.Error);
    }

    [Fact]
    public async Task ExchangeCurrency_IfCurrenciesAreSame_ReturnsSameAmount()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "USD", ToCurrency = "USD" };

        var currencies = new List<string> { currencyExchangeQuery.FromCurrency};
        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
           .Returns(1);

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        Assert.True(result.Success);
        Assert.Equal(currencyExchangeQuery.Amount, result.Value);
    }

    [Theory]
    [InlineData("EUR", "DKK", 100, 743.94)]
    [InlineData("JPY", "DKK", 100, 5.97)]
    [InlineData("SEK", "DKK", 100, 76.10)]
    public async Task ExchangeCurrency_FromForeignCurrencyToDKK_ReturnsCorrectResult(string currencyFrom, string currencyTo, decimal amount, decimal expectedResult)
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = amount, FromCurrency = currencyFrom, ToCurrency = currencyTo };
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency };

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
           .Returns(2);

        _currencyRateRepository
           .GetCurrencyExchangeRateAsync(currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency, Arg.Any<CancellationToken>())
           .Returns(new CurrencyRate {
               MainCurrency = currencyExchangeQuery.FromCurrency,
               MoneyCurrency = currencyExchangeQuery.ToCurrency,
               Rate = CurrencyRates.First(r => r.MainCurrency == currencyExchangeQuery.FromCurrency && r.MoneyCurrency == currencyExchangeQuery.ToCurrency).Rate
           });

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        Assert.Equal(expectedResult, result.Value);
    }

    [Theory]
    [InlineData("DKK", "EUR", 100, 13.44)]
    [InlineData("DKK", "JPY", 100, 1673.92)]
    [InlineData("DKK", "SEK", 100, 131.41)]
    public async Task ExchangeCurrency_FromDKKToForeignCurrency_ReturnsCorrectResult(string currencyFrom, string currencyTo, decimal amount, decimal expectedResult)
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = amount, FromCurrency = currencyFrom, ToCurrency = currencyTo };
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency };

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
           .Returns(2);

        _currencyRateRepository
           .GetCurrencyExchangeRateAsync(currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency, Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<CurrencyRate?>(null));

        _currencyRateRepository
           .GetAllCurrencyExchangeRatesAsync(Arg.Any<CancellationToken>())
           .Returns(CurrencyRates);

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        Assert.Equal(expectedResult, result.Value);
    }

    [Theory]
    [InlineData("EUR", "USD", 10, 11.22)]
    [InlineData("EUR", "JPY", 10, 1245.30)]
    [InlineData("NOK", "CHF", 10, 1.15)]
    [InlineData("EUR", "LTU", 10, 360.66)]
    public async Task ExchangeCurrency_IfNoDirectRatesAreInList_ReturnsCorrectResult(string currencyFrom, string currencyTo, decimal amount, decimal expectedResult)
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = amount, FromCurrency = currencyFrom, ToCurrency = currencyTo };
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency };

        _currencyRateRepository
           .GetCurrencyExchangeRateAsync(currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency, Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<CurrencyRate?>(null));

        _currencyRepository
          .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
          .Returns(2);

        _currencyRateRepository
           .GetAllCurrencyExchangeRatesAsync(Arg.Any<CancellationToken>())
           .Returns(CurrencyRates);

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        Assert.Equal(expectedResult, result.Value);
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
            new CurrencyRate { MainCurrency = "LTU", MoneyCurrency = "JPY", Rate = 3.4528m }
        };
}
