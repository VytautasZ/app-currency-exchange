namespace CurrencyExchange.Shared.CurrencyResult;

public class ExchangeResult<T>
{
    public bool Success { get; }
    public T Value { get; }
    public string Error { get; }

    private ExchangeResult(bool success, T value, string error)
    {
        Success = success;
        Value = value;
        Error = error;
    }

    public static ExchangeResult<T> Ok(T value) => new ExchangeResult<T>(true, value, null);
    public static ExchangeResult<T> Fail(string error) => new ExchangeResult<T>(false, default(T), error);
}
