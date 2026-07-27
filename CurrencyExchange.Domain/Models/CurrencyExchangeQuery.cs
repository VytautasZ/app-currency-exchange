namespace CurrencyExchange.Domain.Models;

public class CurrencyExchangeQuery
{
    public required string FromCurrency { get; set; }
    public required string ToCurrency { get; set; }
    public required decimal Amount { get; set; }
}
