namespace CurrencyExchange.Domain.Models;

public class Currency
{
    public required string CurrencyCode { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}
