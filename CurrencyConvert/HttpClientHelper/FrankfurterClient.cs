using CurrencyConvert.Interface;
using Polly;
using Polly.Retry;

namespace CurrencyConvert.HttpClientHelper
{
    public class FrankfurterClient: IFrankfurterClient
    {
        private readonly HttpClient _client; 
      
        public AsyncRetryPolicy<HttpResponseMessage> HttpRetryWithWaiting { get; set; }
        public FrankfurterClient()
        {
            
            _client = new HttpClient();

            HttpRetryWithWaiting = Policy.HandleResult<HttpResponseMessage>(
                response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(5));
            
    }
        public async Task<HttpResponseMessage> GetAsync<T>(string Url)
        {
            var response = await HttpRetryWithWaiting.ExecuteAsync(() =>
             _client.GetAsync(Url));
            return response;
        
        }     

    }
}
