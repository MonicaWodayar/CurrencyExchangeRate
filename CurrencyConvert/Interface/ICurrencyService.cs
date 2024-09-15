using CurrencyConvert.Model;

namespace CurrencyConvert.Interface
{
    public interface ICurrencyService
    {
        Task<bool> GetSupportedCurrenciesAsync(string currenyCode);
        Task<Dictionary<string, decimal>> GetExchangeRateForBaseCurrency(string baseCurrency);
        Task<CurrencyExchange> CurrencyConvertAsync(decimal amount, string from, string to);
        Task<ExchangeBaseApiResponse> CurrencyConvertByDateAsync(DateTime startDate, DateTime endDate, string from, int page, int pageSize);
    }
}
