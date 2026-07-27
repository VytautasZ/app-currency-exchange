using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.ConsoleUI.Mapper;

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

            var exchangeQuery = Console.ReadLine();

            if (!CurrencyExchangeUserInputValidator.Validate(exchangeQuery))
            {
                Console.WriteLine("Could not read query. Use the format: Exchange USD/EUR 100\n");
                continue;
            }

            var exchangeResult = await _currencyExchangeService.ExchangeCurrencyAsync(exchangeQuery.ToCurrencyExchangeQuery(), cancellationToken: default);

            if (!exchangeResult.Success)
            {
                Console.WriteLine($"{exchangeResult.Error}. Please try again.\n");
                continue;
            }

            Console.WriteLine($"{exchangeResult.Value}\n");

            Console.WriteLine("------------------------\n");

            Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");

            if (Console.ReadLine() == "n")
            {
                endApp = true;
            }

            Console.WriteLine("\n");
        }
        return;
    }
}
