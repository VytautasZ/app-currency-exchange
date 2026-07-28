using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Application.Services;
using CurrencyExchange.Domain.Models;
using CurrencyExchange.Shared.CurrencyResult;
using NSubstitute;

namespace CurrencyConverter.Application.UnitTests;

public class CurrencyExchangeManagerTests
{
    private readonly ICurrencyExchangeService _currencyExchangeService = Substitute.For<ICurrencyExchangeService>();
    private readonly ICurrencyRepository _currencyRepository = Substitute.For<ICurrencyRepository>();
    private readonly CurrencyExchangeManager _sut;

    public CurrencyExchangeManagerTests()
    {
        _sut = new CurrencyExchangeManager(_currencyExchangeService, _currencyRepository);
    }

    [Fact]
    public async Task ProceedCurrencyExchange_IfOneCurrencyIsUnknown_ReturnsErrorMessage()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "USD", ToCurrency = "ABC" };
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency };

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
           .Returns(1);

        //Act
        var result = await _sut.ProceedCurrencyExchangeAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Unknown currency", result.Error);
    }

    [Fact]
    public async Task ProceedCurrencyExchange_IfBothCurrenciesAreUnknown_ReturnsErrorMessage()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "ABC", ToCurrency = "XYZ" };
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency };

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
           .Returns(0);

        //Act
        var result = await _sut.ProceedCurrencyExchangeAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Unknown currency", result.Error);
    }

    [Fact]
    public async Task ProceedCurrencyExchange_IfCurrencyIsUnknown_DoesNotCallTheExchangeService()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "USD", ToCurrency = "ABC" };

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
           .Returns(1);

        //Act
        await _sut.ProceedCurrencyExchangeAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        await _currencyExchangeService
            .DidNotReceive()
            .ExchangeCurrencyAsync(Arg.Any<CurrencyExchangeQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProceedCurrencyExchange_IfBothCurrenciesExist_ReturnsTheExchangeServiceResult()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "EUR", ToCurrency = "DKK" };
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency };

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Is<IEnumerable<string>>(c => c != null && c.SequenceEqual(currencies)), Arg.Any<CancellationToken>())
           .Returns(2);

        _currencyExchangeService
            .ExchangeCurrencyAsync(currencyExchangeQuery, Arg.Any<CancellationToken>())
            .Returns(ExchangeResult<decimal>.Ok(743.94m));

        //Act
        var result = await _sut.ProceedCurrencyExchangeAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        Assert.True(result.Success);
        Assert.Equal(743.94m, result.Value);
    }

    [Fact]
    public async Task ProceedCurrencyExchange_IfBothCurrenciesExist_PassesTheQueryToTheExchangeService()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "EUR", ToCurrency = "DKK" };

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
           .Returns(2);

        //Act
        await _sut.ProceedCurrencyExchangeAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        await _currencyExchangeService
            .Received(1)
            .ExchangeCurrencyAsync(currencyExchangeQuery, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProceedCurrencyExchange_IfCurrenciesAreSame_AsksTheRepositoryForOneCurrency()
    {
        //Arange
        var currencyExchangeQuery = new CurrencyExchangeQuery() { Amount = 100, FromCurrency = "USD", ToCurrency = "USD" };

        _currencyRepository
           .GetCountOfExistingCurrienciesAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
           .Returns(1);

        //Act
        var result = await _sut.ProceedCurrencyExchangeAsync(currencyExchangeQuery, cancellationToken: default);

        //Assert
        await _currencyRepository
            .Received(1)
            .GetCountOfExistingCurrienciesAsync(
                Arg.Is<IEnumerable<string>>(c => c != null && c.Count() == 1 && c.Single() == "USD"),
                Arg.Any<CancellationToken>());
    }
}
