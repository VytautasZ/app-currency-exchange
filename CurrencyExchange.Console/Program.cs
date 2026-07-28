using CurrencyExchange.Application;
using CurrencyExchange.ConsoleUI.DependencyInjection;
using CurrencyExchange.ConsoleUI.Services;
using CurrencyExchange.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

const string ConnectionName = "DefaultConnection";

var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: false)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString(ConnectionName);

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        $"Connection string '{ConnectionName}' is not configured. Set it with:{Environment.NewLine}" +
        $"  dotnet user-secrets set \"ConnectionStrings:{ConnectionName}\" \"<connection string>\"{Environment.NewLine}" +
        $"or via the ConnectionStrings__{ConnectionName} environment variable.");
}

var services = new ServiceCollection();
services.AddConsoleServices();
services.AddApplicationServices();
services.AddInfrastructureServices(connectionString);

await using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateScopes = true,
    ValidateOnBuild = true
});

await provider.MigrateDatabaseAsync();

using var scope = provider.CreateScope();
var consoleService = scope.ServiceProvider.GetRequiredService<IConsoleUIService>();

await consoleService.RunCurrencyExchangeConsoleUIAsync();
