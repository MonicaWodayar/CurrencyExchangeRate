namespace CurrencyConvert.Interface
{
    public interface IFrankfurterClient
    {
        Task<HttpResponseMessage> GetAsync<T>(string Url);
    }
}
