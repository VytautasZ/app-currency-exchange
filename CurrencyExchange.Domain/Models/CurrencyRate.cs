namespace CurrencyExchange.Domain.Models;

public class CurrencyRate
{
    public required string MainCurrency { get; set; }
    public required string MoneyCurrency { get; set; }
    public required decimal Rate { get; set; }
}
