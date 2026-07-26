using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Infrastructure.Persistence;
using CurrencyExchange.Infrastructure.Repositories;
using CurrencyExchange.Interfaces.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyExchange.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CurrencyExchangeDbContext>(options =>
            options.UseSqlServer(connectionString, sqlServer =>
                sqlServer.MigrationsAssembly(typeof(CurrencyExchangeDbContext).Assembly.FullName)));

        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<ICurrencyExchangeRateRepository, CurrencyExchangeRateRepository>();

        return services;
    }
}
