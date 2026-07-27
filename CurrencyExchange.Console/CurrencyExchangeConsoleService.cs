using CurrencyExchange.Application.Interfaces;

namespace CurrencyExchange.ConsoleUI;

internal class CurrencyExchangeConsoleService : ICurrencyExchangeConsoleService
{
    private readonly ICurrencyExchangeService _currencyExchangeService;

    public CurrencyExchangeConsoleService(ICurrencyExchangeService currencyExchangeService)
    {
        _currencyExchangeService = currencyExchangeService;
    }

    public void RunCurrencyExchangeApp()
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
