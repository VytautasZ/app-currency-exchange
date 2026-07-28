# app-currency-exchange

A console application that converts an amount between currencies. Rates are stored in SQL Server; pairs without a direct rate are converted by routing through intermediate currencies (breadth-first search over a rate graph, e.g. `EUR -> DKK -> JPY -> LTU`).

## Solution layout

| Project | Purpose |
|---|---|
| `CurrencyExchange.Domain` | Domain models: `Currency`, `CurrencyRate`, `CurrencyExchangeQuery` |
| `CurrencyExchange.Application` | `CurrencyExchangeService` (conversion logic) and repository interfaces |
| `CurrencyExchange.Infrastructure` | EF Core `DbContext`, entity configurations, migrations, seed data, repository implementations |
| `CurrencyExchange.Shared` | Cross-cutting types (`ExchangeResult<T>`) |
| `CurrencyExchange.Console` (`CurrencyExchange.ConsoleUI`) | Console entry point: input parsing, validation, DI composition root |
| `CurrencyConverter.Application.UnitTests` | xunit tests (NSubstitute for repository mocks) |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for the SQL Server container)

## Getting started

1. Start the database:

   ```bash
   docker compose up -d
   ```

   This runs SQL Server 2022 in a container named `currencyexchange-db` with a named volume for persistence, and waits until it reports healthy.

2. Run the application:

   ```bash
   dotnet run --project CurrencyExchange.Console/CurrencyExchange.ConsoleUI.csproj
   ```

   On startup the app applies any pending EF Core migrations, creating the `CurrencyExchange` database and seeding 9 currencies and 8 exchange rates on first run.

3. Enter a query at the prompt:

   ```text
   Exchange USD/EUR 100
   ```

   Format: `Exchange <FROM>/<TO> <amount>` — three-letter currency codes, positive amount with up to 6 decimal places. The result is printed rounded to two decimal places.

Seeded currencies: DKK (pivot), EUR, USD, GBP, SEK, NOK, CHF, JPY, LTU (quoted against JPY).

## Configuration

Configuration is environment-based: `appsettings.{DOTNET_ENVIRONMENT}.json`, then user secrets, then environment variables (later sources win). `DOTNET_ENVIRONMENT` defaults to `Development`.

The connection string is read from `ConnectionStrings:DefaultConnection`. For local development it lives in `CurrencyExchange.Console/appsettings.Development.json` and must match the SA password in `docker-compose.yml` — if you change one, change the other. Alternatives:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection string>" --project CurrencyExchange.Console/CurrencyExchange.ConsoleUI.csproj
```

or the `ConnectionStrings__DefaultConnection` environment variable.

## Database and migrations

Migrations live in `CurrencyExchange.Infrastructure/Persistence/Migrations` and are applied automatically at application startup. To run them manually:

```bash
dotnet ef database update --project CurrencyExchange.Infrastructure/CurrencyExchange.Infrastructure.csproj
```

The design-time factory reads the `CURRENCYEXCHANGE_CONNECTIONSTRING` environment variable, falling back to LocalDB. Seed data is embedded in the initial migration (`HasData`).

To reset the database completely:

```bash
docker compose down -v
```

## Tests

```bash
dotnet test
```

Covers the exchange service (validation, direct rates, inverse rates, multi-hop cross rates) and console input parsing.

## Contributing

Branch naming, commit message format and workflow rules for this repository are described in [AGENTS.md](AGENTS.md).
