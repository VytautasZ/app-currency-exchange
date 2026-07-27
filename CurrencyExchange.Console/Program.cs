using CurrencyExchange.Application;
using CurrencyExchange.ConsoleUI;
using CurrencyExchange.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddConsoleServices();
services.AddApplicationServices();
services.AddInfrastructureServices("DefaultConnection");

await using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateScopes = true,
    ValidateOnBuild = true
});

using var scope = provider.CreateScope();
var consoleService = scope.ServiceProvider.GetRequiredService<ICurrencyExchangeConsoleService>();

consoleService.RunCurrencyExchangeApp();
