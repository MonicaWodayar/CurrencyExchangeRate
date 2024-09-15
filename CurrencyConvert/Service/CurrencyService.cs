
using CurrencyConvert.Helper;
using CurrencyConvert.HttpClientHelper;
using CurrencyConvert.Interface;
using CurrencyConvert.Model;
using Flurl;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CurrencyConvert.Service
{
    public class CurrencyService : ICurrencyService
    {
        protected readonly Url CurrencyEndpoint;
        public IFrankfurterClient frankfurterClient;
        public CurrencyService()
        {
            CurrencyEndpoint = ConfigurationHelper.GetBaseURL();
            frankfurterClient = new FrankfurterClient();
        }

        public async Task<bool> GetSupportedCurrenciesAsync(string currenyCode)
        {
            RefreshEndpoint();
            if (SupportedCurrencyHelper.SupportedCurrency==null ||SupportedCurrencyHelper.SupportedCurrency.Count == 0)
            {
                CurrencyEndpoint.AppendPathSegment("currencies");

                var responseData = await GetAsync<JsonObject>()
                    .ConfigureAwait(false);
                var response = responseData.Content.ReadAsStringAsync().Result;
                if (responseData.IsSuccessStatusCode)
                {
                    var currencies = JsonSerializer.Deserialize<Dictionary<string, string>>(response);
                    if (currencies != null)
                    {
                        SupportedCurrencyHelper.SupportedCurrency = currencies.Keys.ToList();
                        return SupportedCurrencyHelper.SupportedCurrency.Contains(currenyCode.ToUpper()) ? true : false;
                    }
                }
                //Retruning true since any failure might occur and still pass when tried with normal api
                return true;
            }
            else
            {
               return SupportedCurrencyHelper.SupportedCurrency.Contains(currenyCode.ToUpper())?true : false;
            }        
        }
        public async Task<Dictionary<string, decimal>> GetExchangeRateForBaseCurrency(string baseCurrency)
        {
            RefreshEndpoint();
            CurrencyEndpoint.AppendPathSegment("latest");
            CurrencyEndpoint.SetQueryParam("base", baseCurrency);


            var responseData = await GetAsync<ExchangeBaseApiResponse>().ConfigureAwait(false);
            var response = responseData.Content.ReadAsStringAsync().Result;
            if (responseData.IsSuccessStatusCode)
            {
                var jsonObject = JObject.Parse(response);

                // Extract the 'rates' object
                var rates = jsonObject["rates"];
                if (rates != null)
                {
                    return rates.ToObject<Dictionary<string, decimal>>();
                }
            }
            else
            {
                throw new Exception(response);
            }
            throw new Exception("Data Error");
        }

        public async Task<CurrencyExchange> CurrencyConvertAsync(decimal amount, string from, string to)
        {
            RefreshEndpoint();
            CurrencyEndpoint.AppendPathSegment("latest");
            if (amount > decimal.Zero) CurrencyEndpoint.SetQueryParam("amount", amount);
            CurrencyEndpoint.SetQueryParam("from", from.ToString());
            CurrencyEndpoint.SetQueryParam("to", to.ToString());


            var responseData = await GetAsync<CurrencyExchange>().ConfigureAwait(false);
            var response = responseData.Content.ReadAsStringAsync().Result;
            if (responseData.IsSuccessStatusCode)
            {
                CurrencyExchange exchange = JsonSerializer.Deserialize<CurrencyExchange>(response);
                return exchange;
            }
            else
            {
                throw new Exception(response);
            }
        }

        public async Task<ExchangeBaseApiResponse> CurrencyConvertByDateAsync(DateTime startDate, DateTime endDate, string from, int page, int pageSize)
        {
            RefreshEndpoint();
            CurrencyEndpoint.AppendPathSegment("/");
            CurrencyEndpoint.AppendPathSegment(CurrencyDataHelper.DateIntervalToString(startDate, endDate));
            CurrencyEndpoint.SetQueryParam("from", from.ToString());
            var responseData = await GetAsync<ExchangeBaseApiResponse>().ConfigureAwait(false);
            var response = responseData.Content.ReadAsStringAsync().Result;
            if (responseData.IsSuccessStatusCode)
            {
                if (response != null)
                {
                    ExchangeBaseApiResponse exchange = JsonSerializer.Deserialize<ExchangeBaseApiResponse>(response);
                    if (exchange != null)
                    {
                        if (page > exchange.Rates.Count || pageSize > exchange.Rates.Count)
                        {
                            page = 1;
                            pageSize = exchange.Rates.Count;
                        }
                        var paginatedRates = exchange.Rates.Skip((page - 1) * pageSize).Take(pageSize).ToDictionary(entry => entry.Key, entry => entry.Value);
                        if (paginatedRates != null)
                        {
                            var paginatedResponse = new ExchangeBaseApiResponse
                            {
                                Amount = exchange.Amount,
                                StartDate = exchange.StartDate,
                                EndDate = exchange.EndDate,
                                Rates = paginatedRates
                            };
                            return paginatedResponse;
                        }
                    }
                }

            }
            else
            {
                throw new Exception(response);
            }
            throw new Exception("Data Not Found");
        }



        protected Task<HttpResponseMessage> GetAsync<T>()
        {
            return frankfurterClient.GetAsync<T>(CurrencyEndpoint.ToString());
        }
        private void RefreshEndpoint()
        {
            CurrencyEndpoint.Reset();
        }
    }
}
