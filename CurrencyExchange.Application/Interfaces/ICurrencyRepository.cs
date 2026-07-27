namespace CurrencyExchange.Application.Interfaces;

public interface ICurrencyRepository
{
    public Task<bool> CurrencyExistsAsync(IEnumerable<string> currencyCodes, CancellationToken cancellationToken);
}
