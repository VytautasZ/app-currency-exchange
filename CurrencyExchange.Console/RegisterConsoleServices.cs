using Microsoft.Extensions.DependencyInjection;

namespace CurrencyExchange.ConsoleUI;

public static class RegisterConsoleServices
{
    public static IServiceCollection AddConsoleServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyExchangeConsoleService, CurrencyExchangeConsoleService>();

        return services;
    }
}
