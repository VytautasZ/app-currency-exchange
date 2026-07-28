namespace CurrencyExchange.Application.Interfaces;

public interface ICurrencyRepository
{
    public Task<int> GetCountOfExistingCurrienciesAsync(IEnumerable<string> currencyCodes, CancellationToken cancellationToken);
}
