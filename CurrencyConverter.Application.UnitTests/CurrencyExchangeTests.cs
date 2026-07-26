using CurrencyExchange.Domain.Models;
using CurrencyExchange.Interfaces.Application;
using CurrencyExchange.Shared.CurrencyResult;
using NSubstitute;

namespace CurrencyConverter.Application.UnitTests;

public class CurrencyExchangeTests
{
    private readonly ICurrencyRateRepository _currencyRateRepository = Substitute.For<ICurrencyRateRepository>();
    private readonly CurrencyExchangeService _sut;

    public CurrencyExchangeTests()
    {
        _sut = new CurrencyExchangeService(_currencyRateRepository);
    }

    [Fact]
    public async Task ExchangeCurrency_IfCurrencyUnknown_ReturnsErrorMessage()
    {
        var currencyFrom = "USD";
        var currencyTo = "ABC";

        //Arange
        _currencyRateRepository
           .CurrencyExistsAsync(currencyFrom, Arg.Any<CancellationToken>())
           .Returns(true);

        _currencyRateRepository
           .CurrencyExistsAsync(currencyTo, Arg.Any<CancellationToken>())
           .Returns(false);


        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyFrom, currencyTo, 100);

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Unknown currency", result.Error);
    }

    [Fact]
    public async Task ExchangeCurrency_IfCurrenciesAreSame_ReturnsSameAmount()
    {
        //Arange
        var amount = 591;

        //Act
        var result = await _sut.ExchangeCurrencyAsync("USD", "USD", amount);

        //Assert
        Assert.True(result.Success);
        Assert.Equal(amount, result.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task ExchangeCurrency_IfAmountIsNegativeOrZero_ReturnsError(decimal amount)
    {
        //Arange

        //Act
        var result = await _sut.ExchangeCurrencyAsync("USD", "DKK", amount);

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid amount", result.Error);
    }

    [Theory]
    [InlineData("EUR", "DKK", 100, 743.94)]
    [InlineData("JPY", "DKK", 100, 5.97)]
    [InlineData("SEK", "DKK", 100, 76.10)]
    public async Task ExchangeCurrency_FromForeignCurrencyToDKK_ReturnsCorrectResult(string currencyFrom, string currencyTo, decimal amount, decimal expectedResult)
    {
        //Arange
        _currencyRateRepository
           .CurrencyExistsAsync(currencyFrom, Arg.Any<CancellationToken>())
           .Returns(true);

        _currencyRateRepository
           .CurrencyExistsAsync(currencyTo, Arg.Any<CancellationToken>())
           .Returns(true);

        _currencyRateRepository
           .GetCurrencyRateAsync(currencyFrom, currencyTo, Arg.Any<CancellationToken>())
           .Returns(new CurrencyRate { MainCurrency = currencyFrom, MoneyCurrency = currencyTo,
               Rate = CurrencyRates.First(r => r.MainCurrency == currencyFrom && r.MoneyCurrency == currencyTo).Rate });

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyFrom, currencyTo, amount);

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
        _currencyRateRepository
           .GetCurrencyRateAsync(currencyFrom, currencyTo, Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<CurrencyRate?>(null));

        _currencyRateRepository
           .CurrencyExistsAsync(currencyFrom, Arg.Any<CancellationToken>())
           .Returns(true);

        _currencyRateRepository
           .CurrencyExistsAsync(currencyTo, Arg.Any<CancellationToken>())
           .Returns(true);

        _currencyRateRepository
           .GetAllCurrencyRatesAsync(Arg.Any<CancellationToken>())
           .Returns(CurrencyRates);

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyFrom, currencyTo, amount);

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
        _currencyRateRepository
           .GetCurrencyRateAsync(currencyFrom, currencyTo, Arg.Any<CancellationToken>())
           .Returns(Task.FromResult<CurrencyRate?>(null));

        _currencyRateRepository
          .CurrencyExistsAsync(currencyFrom, Arg.Any<CancellationToken>())
          .Returns(true);

        _currencyRateRepository
          .CurrencyExistsAsync(currencyTo, Arg.Any<CancellationToken>())
           .Returns(true);

        _currencyRateRepository
           .GetAllCurrencyRatesAsync(Arg.Any<CancellationToken>())
           .Returns(CurrencyRates);

        //Act
        var result = await _sut.ExchangeCurrencyAsync(currencyFrom, currencyTo, amount);

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

    private  class CurrencyExchangeService
    {
        private readonly ICurrencyRateRepository _currencyRateRepository;

        public CurrencyExchangeService(ICurrencyRateRepository currencyRateRepository)
        {
            _currencyRateRepository = currencyRateRepository;
        }

        internal async Task<ExchangeResult<decimal>> ExchangeCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            if (amount <= 0)
            {
                return ExchangeResult<decimal>.Fail("Invalid amount");
            }

            if (fromCurrency == toCurrency)
            {
                return ExchangeResult<decimal>.Ok(amount);
            }

            if (!(await CurrencyExists(fromCurrency)) || !(await CurrencyExists(toCurrency)))   
            {
                return ExchangeResult<decimal>.Fail("Unknown currency");
            }

            var rate = await _currencyRateRepository.GetCurrencyRateAsync(fromCurrency, toCurrency, cancellationToken: default);
            if (rate == null)
            {
                var currencyRates = await _currencyRateRepository.GetAllCurrencyRatesAsync(cancellationToken: default);

                var calculatedRateResult = CalculteCrossRate(fromCurrency, toCurrency, currencyRates);

                if(!calculatedRateResult.Success)
                {
                    return calculatedRateResult;
                }

                return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(calculatedRateResult.Value, amount));
            }

            return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(rate.Rate, amount));
        }

        private async Task<bool> CurrencyExists(string currency)
        {
            return await _currencyRateRepository.CurrencyExistsAsync(currency, cancellationToken: default);
        }

        private static ExchangeResult<decimal> CalculteCrossRate(string fromCurrency, string toCurrency, IEnumerable<CurrencyRate> currencyRates)
        {
            var graph = BuildCurrencyGraph(currencyRates);

            var queue = new Queue<(string Currency, decimal InitialCurrency)>();
            var visited = new HashSet<string> { fromCurrency };

            queue.Enqueue((fromCurrency, 1m));

            while (queue.Count > 0)
            {
                var (currency, initialCurrency) = queue.Dequeue();

                foreach (var (nextCurrency, rate) in graph[currency])
                {
                    if (!visited.Add(nextCurrency))     
                    {
                        continue;
                    }

                    var calculatedRate = initialCurrency * rate;

                    if (nextCurrency == toCurrency)
                    {
                        return ExchangeResult<decimal>.Ok(calculatedRate);
                    }

                    queue.Enqueue((nextCurrency, calculatedRate));
                }
            }

            return ExchangeResult<decimal>.Fail("No rate was calculated");
        }

        private static Dictionary<string, Dictionary<string, decimal>> BuildCurrencyGraph(IEnumerable<CurrencyRate> currencyRates)
        {
            var graph = new Dictionary<string, Dictionary<string, decimal>>();

            void AddEdge(string from, string to, decimal rate)
            {
                if (!graph.TryGetValue(from, out var edges))
                {
                    edges = new Dictionary<string, decimal>();
                    graph[from] = edges;
                }

                edges[to] = rate;
            }

            foreach (var rate in currencyRates)
            {
                AddEdge(rate.MainCurrency, rate.MoneyCurrency, rate.Rate);
                AddEdge(rate.MoneyCurrency, rate.MainCurrency, 1m / rate.Rate);
            }

            return graph;
        }

        private static decimal CalculateRoundedExchangedAmount(decimal rate, decimal amount)
        {
            var result = amount * rate;
            return RoundToTwoDecimalPlaces(result);
        }

        private static decimal RoundToTwoDecimalPlaces(decimal value)
        {
            return Math.Round(value, 2);
        }
    }
}
