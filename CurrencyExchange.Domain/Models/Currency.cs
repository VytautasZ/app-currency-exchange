namespace CurrencyExchange.Domain.Models;

public class Currency
{
    public Guid Id { get; set; }
    public required string CurrencyCode { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}
