using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.ConsoleUI.Helpers;

namespace CurrencyExchange.ConsoleUI.Services;

internal class CurrencyExchangeConsoleService : ICurrencyExchangeConsoleService
{
    private readonly ICurrencyExchangeService _currencyExchangeService;

    public CurrencyExchangeConsoleService(ICurrencyExchangeService currencyExchangeService)
    {
        _currencyExchangeService = currencyExchangeService;
    }

    public async Task RunCurrencyExchangeAppAsync()
    {
        var endApp = false;
        Console.WriteLine("Currency Exchange Calculator\r");
        Console.WriteLine("------------------------\n");

        while (!endApp)
        {
            Console.WriteLine("Type a currency exchange query in given format: ");
            Console.WriteLine("Exchange <currency pair> <amount to change> ");
            Console.WriteLine("Example: Exchange USD/EUR 100 ");

            var exchangeQueryRaw = Console.ReadLine();
            var exchangeQuery = CurrencyExchangeQueryParser.ParseQueryString(exchangeQueryRaw);

            if (!exchangeQuery.Success)
            {
                Console.WriteLine($"{exchangeQuery.Error}\n");
                continue;
            }

            var exchangeResult = await _currencyExchangeService.ExchangeCurrencyAsync(exchangeQuery.Value, cancellationToken: default);

            if (!exchangeResult.Success)
            {
                Console.WriteLine($"{exchangeResult.Error}. Please try again.\n");
                continue;
            }

            Console.WriteLine($"{exchangeResult.Value}\n");
            Console.WriteLine("------------------------\n");
            Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");

            var exitInput = Console.ReadLine();

            if (exitInput is null || exitInput == "n")
            {
                endApp = true;
            }

            Console.WriteLine("\n");
        }
        return;
    }
}
