using CurrencyExchange.ConsoleUI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyExchange.ConsoleUI.DependencyInjection;

public static class RegisterConsoleUIServices
{
    public static IServiceCollection AddConsoleServices(this IServiceCollection services)
    {
        services.AddScoped<IConsoleUIService, ConsoleUIService>();

        return services;
    }
}
