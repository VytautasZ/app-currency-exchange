using CurrencyExchange.Domain.Models;
using CurrencyExchange.Shared.CurrencyResult;

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
        Assert.False(result.Success);
        Assert.Equal("Unknown currency", result.Error);
    }

    [Fact]
    public void ExchangeCurrency_IfCurrenciesAreSame_ReturnsSameAmount()
    {
        //Arange
        var amount = 591;

        //Act
        var result = CurrrencyExchangeService.ExchangeCurrency("USD", "USD", amount);

        //Assert
        Assert.True(result.Success);
        Assert.Equal(amount, result.Value);
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
        Assert.False(result.Success);
        Assert.Equal("Invalid amount", result.Error);
    }

    [Theory]
    [InlineData("EUR", "DKK", 100, 743.94)]
    [InlineData("JPY", "DKK", 100, 5.97)]
    [InlineData("SEK", "DKK", 100, 76.10)]
    public void ExchangeCurrency_FromForeignCurrencyToDKK_ReturnsCorrectResult(string currencyFrom, string currencyTo, decimal amount, decimal expectedResult)
    {
        //Arange

        //Act
        var result = CurrrencyExchangeService.ExchangeCurrency(currencyFrom, currencyTo, amount);

        //Assert
        Assert.Equal(expectedResult, result.Value);
    }

    [Theory]
    [InlineData("DKK", "EUR", 100, 13.44)]
    [InlineData("DKK", "JPY", 100, 1673.92)]
    [InlineData("DKK", "SEK", 100, 131.41)]
    public void ExchangeCurrency_FromDKKToForeignCurrency_ReturnsCorrectResult(string currencyFrom, string currencyTo, decimal amount, decimal expectedResult)
    {
        //Arange

        //Act
        var result = CurrrencyExchangeService.ExchangeCurrency(currencyFrom, currencyTo, amount);

        //Assert
        Assert.Equal(expectedResult, result.Value);
    }

    [Theory]
    [InlineData("EUR", "USD", 10, 11.22)]
    [InlineData("EUR", "JPY", 10, 1245.30)]
    [InlineData("NOK", "CHF", 10, 1.15)]
        public void ExchangeCurrency_IfNoDirectRatesAreInList_ReturnsCorrectResult(string currencyFrom, string currencyTo, decimal amount, decimal expectedResult)
    {
        //Arange

        //Act
        var result = CurrrencyExchangeService.ExchangeCurrency(currencyFrom, currencyTo, amount);

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
            new CurrencyRate { MainCurrency = "JPY", MoneyCurrency = "DKK", Rate = 0.059740m }
        };

    private static class CurrrencyExchangeService
    {
        internal static ExchangeResult<decimal> ExchangeCurrency(string fromCurrency, string toCurrency, decimal amount)
        {
            if (amount <= 0)
            {
                return ExchangeResult<decimal>.Fail("Invalid amount");
            }

            if (fromCurrency == toCurrency)
            {
                return ExchangeResult<decimal>.Ok(amount);
            }

            if (!CurrencyExists(fromCurrency) || !CurrencyExists(toCurrency))
            {
                return ExchangeResult<decimal>.Fail("Unknown currency");
            }

            var rate = CurrencyRates.FirstOrDefault(cr => cr.MainCurrency == fromCurrency && cr.MoneyCurrency == toCurrency);
            if (rate == null)
            {
                return CalculteCrossRate(fromCurrency, toCurrency, amount);
            }

            return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(rate.Rate, amount));
        }

        private static bool CurrencyExists(string currency)
        {
            return CurrencyRates.Any(cr => cr.MainCurrency == currency || cr.MoneyCurrency == currency);
        }

        private static ExchangeResult<decimal> CalculteCrossRate(string fromCurrency, string toCurrency, decimal amount)
        {
            var relevantCurrencyRates = CurrencyRates
                .Where(cr => cr.MainCurrency == fromCurrency || cr.MoneyCurrency == fromCurrency
                          || cr.MainCurrency == toCurrency || cr.MoneyCurrency == toCurrency)
                .ToList();

            var queue = new Queue<(string Currency, decimal Factor)>();
            var visited = new HashSet<string> { fromCurrency };
            var graph = BuildCurrencyGraph(relevantCurrencyRates);

            queue.Enqueue((fromCurrency, 1m));

            while (queue.Count > 0)
            {
                var (currency, factor) = queue.Dequeue();

                foreach (var (next, rate) in graph[currency])
                {
                    if (!visited.Add(next))     
                    {
                        continue;
                    }

                    var nextFactor = factor * rate;

                    if (next == toCurrency)
                    {
                        return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(nextFactor, amount));
                    }

                    queue.Enqueue((next, nextFactor));
                }
            }

            return ExchangeResult<decimal>.Fail("No result was calculated");
        }

        private static Dictionary<string, Dictionary<string, decimal>> BuildCurrencyGraph(List<CurrencyRate> currencyRates)
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
