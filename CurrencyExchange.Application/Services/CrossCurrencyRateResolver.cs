using CurrencyExchange.Domain.Models;

namespace CurrencyExchange.Application.Services;

public static class CrossCurrencyRateResolver
{
    public static decimal? ResolveExchangeRateAsync(string fromCurrency, string toCurrency, IEnumerable<CurrencyRate> currencyRates, CancellationToken cancellationToken)
    {
        if(fromCurrency == toCurrency)
        {
            return 1m;
        }

        var graph = BuildCurrencyGraph(currencyRates);
        var calculatedExchangeRate = FindCurrencyRateFromGraph(graph, fromCurrency, toCurrency);

        return calculatedExchangeRate;
    }

    private static Dictionary<string, Dictionary<string, decimal>> BuildCurrencyGraph(IEnumerable<CurrencyRate> currencyRates)
    {
        var graph = new Dictionary<string, Dictionary<string, decimal>>();

        foreach (var rate in currencyRates)
        {
            if (rate.Rate <= 0)
            {
                continue;
            }

            AddEdge(graph, rate.MainCurrency, rate.MoneyCurrency, rate.Rate);
            AddEdge(graph, rate.MoneyCurrency, rate.MainCurrency, 1m / rate.Rate);
        }

        return graph;
    }

    private static void AddEdge(Dictionary<string, Dictionary<string, decimal>> graph, string from, string to, decimal rate)
    {
        if (!graph.TryGetValue(from, out var edges))
        {
            edges = new Dictionary<string, decimal>();
            graph[from] = edges;
        }

        edges[to] = rate;
    }

    private static decimal? FindCurrencyRateFromGraph(Dictionary<string, Dictionary<string, decimal>> graph, string fromCurrency, string toCurrency)
    {
        var queue = new Queue<(string Currency, decimal Factor)>();
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
