using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Domain.Models;
using CurrencyExchange.Interfaces.Application;
using CurrencyExchange.Shared.CurrencyResult;

namespace CurrencyExchange.Application.Services;

public class CurrencyExchangeService : ICurrencyExchangeService
{
    private readonly ICurrencyExchangeRateRepository _currencyRateRepository;
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyExchangeService(ICurrencyExchangeRateRepository currencyRateRepository, ICurrencyRepository currencyRepository)
    {
        _currencyRateRepository = currencyRateRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<ExchangeResult<decimal>> ExchangeCurrencyAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken)
    {
        if (currencyExchangeQuery.Amount <= 0)
        {
            return ExchangeResult<decimal>.Fail("Invalid amount");
        }

        if (!await CurrencyExistsAsync(currencyExchangeQuery, cancellationToken))
        {
            return ExchangeResult<decimal>.Fail("Unknown currency");
        }

        if (currencyExchangeQuery.FromCurrency == currencyExchangeQuery.ToCurrency)
        {
            return ExchangeResult<decimal>.Ok(currencyExchangeQuery.Amount);
        }

        var rate = await _currencyRateRepository.GetCurrencyExchangeRateAsync(currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency, cancellationToken);
        if (rate != null)
        {
            return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(rate.Rate, currencyExchangeQuery.Amount));
        }

        var calculatedRateResult = await CalculateExchangeRateAsync(currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency, cancellationToken);
        if (calculatedRateResult != null)
        {
            return ExchangeResult<decimal>.Ok(CalculateRoundedExchangedAmount(calculatedRateResult.Value, currencyExchangeQuery.Amount));
        }

        return ExchangeResult<decimal>.Fail("No rate was calculated");
    }

    private async Task<bool> CurrencyExistsAsync(CurrencyExchangeQuery currencyExchangeQuery, CancellationToken cancellationToken)
    {
        var currencies = new List<string> { currencyExchangeQuery.FromCurrency, currencyExchangeQuery.ToCurrency };
        return await _currencyRepository.CurrencyExistsAsync(currencies, cancellationToken: cancellationToken);
    }

    private static decimal CalculateRoundedExchangedAmount(decimal rate, decimal amount)
    {
        var result = amount * rate;
        return RoundToTwoDecimalPlaces(result);
    }

    private static decimal RoundToTwoDecimalPlaces(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private async Task<decimal?> CalculateExchangeRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken)
    {
        var currencyRates = await _currencyRateRepository.GetAllCurrencyExchangeRatesAsync(cancellationToken);

        if(currencyRates == null || !currencyRates.Any())
        {
            return null;
        }

        var graph = BuildCurrencyGraph(currencyRates);
        var calculatedExchangeRate = TraverseAndCalculateCurencyGraph(graph, fromCurrency, toCurrency);

        return calculatedExchangeRate;
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
            if (rate.Rate <= 0)
            {
                continue;
            }

            AddEdge(rate.MainCurrency, rate.MoneyCurrency, rate.Rate);
            AddEdge(rate.MoneyCurrency, rate.MainCurrency, 1m / rate.Rate);
        }

        return graph;
    }

    private static decimal? TraverseAndCalculateCurencyGraph(Dictionary<string, Dictionary<string, decimal>> graph, string fromCurrency, string toCurrency)
    {
        var queue = new Queue<(string Currency, decimal Factor )>();
        var visited = new HashSet<string> { fromCurrency };

        queue.Enqueue((fromCurrency, 1m));

        while (queue.Count > 0)
        {
            var (currency, factor) = queue.Dequeue();

            foreach (var (nextCurrency, rate) in graph[currency])
            {
                if (!visited.Add(nextCurrency))
                {
                    continue;
                }

                var calculatedRate = factor * rate;

                if (nextCurrency == toCurrency)
                {
                    return calculatedRate;
                }

                queue.Enqueue((nextCurrency, calculatedRate));
            }
        }

        return null;
    }
}
