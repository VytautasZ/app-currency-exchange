namespace CurrencyExchange.Application.Interfaces;

public interface ICurrencyRepository
{
    public Task<bool> CurrencyExistsAsync(string currencyCode, CancellationToken cancellationToken);
}
