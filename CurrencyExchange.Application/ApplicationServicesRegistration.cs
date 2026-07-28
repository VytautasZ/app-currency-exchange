using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace CurrencyExchange.Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyExchangeManager, CurrencyExchangeManager>();
        services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();

        return services;
    }
}
