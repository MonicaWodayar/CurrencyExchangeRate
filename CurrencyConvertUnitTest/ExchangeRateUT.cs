using CurrencyConvert.Controllers;
using CurrencyConvert.Helper;
using CurrencyConvert.Interface;
using CurrencyConvert.Model;
using CurrencyConvert.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;
using System.Text.Json.Nodes;


namespace CurrencyConvertUnitTest
{
    public class ExchangeRateUT: IClassFixture<DatabaseFixture>
    {
        private const string baseUrl = "xx";
        private readonly DatabaseFixture _fixture;
        public ExchangeRateUT(DatabaseFixture fixture)
        {
            _fixture = fixture;
            if (SupportedCurrencyHelper.SupportedCurrency != null) 
            {
                SupportedCurrencyHelper.SupportedCurrency.Clear();
            }
       
            var inMemorySettings = new Dictionary<string, string>
        {
            {"AppSettings:BaseURl", baseUrl},
            {"AppSettings:ExcludedCurrencyCode", "TRY,PLN,THB,MXN"}
            };

            var _configuration = new ConfigurationBuilder()
                  .AddInMemoryCollection(inMemorySettings)
                  .Build();

            // Set the static Configuration property
            ConfigurationHelper.Configuration = _configuration; ;
        }

        [InlineData("")]
        [InlineData("  ")]
        [InlineData("USDD")]

        [Theory]
        public async void GetAllAvailableCurrenciesAsyncInvalidData(string currencyCode)
        {
            var controller = new CurrencyController(new CurrencyService());
            var result = await controller.GetAllAvailableCurrenciesAsync(currencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }


        [InlineData("USD")]

        [Theory]
        public async void GetAllAvailableCurrenciesAsyncValidData(string currencyCode)
        {
            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();

            var supportedCurrencyResponse = "{\"AUD\":\"Australian Dollar\",\"BGN\":\"Bulgarian Lev\",\"USD\":\"United States Dollar\"}";
            HttpResponseMessage SupportedhttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(supportedCurrencyResponse, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<JsonObject>(It.IsAny<string>())).ReturnsAsync(SupportedhttpResponseMessage);

            var response = "{\"amount\":1.0,\"base\":\"USD\",\"date\":\"2024-09-13\",\"rates\":{\"AUD\":1.4928,\"BGN\":1.765}}";
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<ExchangeBaseApiResponse>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetAllAvailableCurrenciesAsync(currencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }


        [InlineData("USD")]

        [Theory]
        public async void GetAllAvailableCurrenciesAsyncValidDataWithPreExistingData(string currencyCode)
        {
            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();

            SupportedCurrencyHelper.SupportedCurrency = new List<string>();
            SupportedCurrencyHelper.SupportedCurrency.Add("USD");

                var response = "{\"amount\":1.0,\"base\":\"USD\",\"date\":\"2024-09-13\",\"rates\":{\"AUD\":1.4928,\"BGN\":1.765}}";
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<ExchangeBaseApiResponse>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetAllAvailableCurrenciesAsync(currencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }
        [InlineData("UDD")]
        [Theory]
        public async void GetAllAvailableCurrenciesAsyncForInvalidCurrencyCode(string currencyCode)
        {
            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();

            var supportedCurrencyResponse = "{\"AUD\":\"Australian Dollar\",\"BGN\":\"Bulgarian Lev\",\"USD\":\"United States Dollar\"}";
            HttpResponseMessage SupportedhttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(supportedCurrencyResponse, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<JsonObject>(It.IsAny<string>())).ReturnsAsync(SupportedhttpResponseMessage);

            var response = "{\"amount\":1.0,\"base\":\"USD\",\"date\":\"2024-09-13\",\"rates\":{\"AUD\":1.4928,\"BGN\":1.765}}";
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<ExchangeBaseApiResponse>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetAllAvailableCurrenciesAsync(currencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }

        [InlineData("USD")]
        [Theory]
        public async void GetAllAvailableCurrenciesAsyncValidDataWithErrorStatus(string currencyCode)
        {
            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();
            var response = "Content Error";

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };


            frankfurterClientmock.Setup(x => x.GetAsync<ExchangeBaseApiResponse>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);
            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetAllAvailableCurrenciesAsync(currencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }

        [InlineData(0,"USD","INR")]
        [InlineData(-1, "USD", "INR")]
        [InlineData(1,"USD","USD")]
        [InlineData(1," ","USD")]
        [InlineData(1, "USD", " ")]
        [InlineData(1, "TRY", "USD")]
        [InlineData(1, "USD", "TRY")]
        [InlineData(1, "USD", "PLN")]
        [InlineData(1, "PLN", "USD")]
        [InlineData(1, "USD", "THB")]
        [InlineData(1, "THB", "USD")]
        [InlineData(1, "USD", "MXN")]
        [InlineData(1, "MXN", "USD")]
        [Theory]
        public async void GetExchangeRatesInvalidData(decimal amount, string fromCurrencyCode, string toCurrencyCode)
        {
            var controller = new CurrencyController(new CurrencyService());
            var result = await controller.GetExchangeRates(amount,fromCurrencyCode,toCurrencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }

        [InlineData(1, "USD", "AUD")]
        [InlineData(0.56, "USD", "AUD")]
        [Theory]
        public async void GetExchangeRatesValidData(decimal amount, string fromCurrencyCode, string toCurrencyCode)
        {
            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();

            var supportedCurrencyResponse = "{\"AUD\":\"Australian Dollar\",\"BGN\":\"Bulgarian Lev\",\"USD\":\"United States Dollar\"}";
            HttpResponseMessage SupportedhttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(supportedCurrencyResponse, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<JsonObject>(It.IsAny<string>())).ReturnsAsync(SupportedhttpResponseMessage);

            var response = "{\"amount\":1.0,\"base\":\"USD\",\"date\":\"2024-09-13\",\"rates\":{\"INR\":83.92}}";
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<CurrencyExchange>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetExchangeRates(amount, fromCurrencyCode, toCurrencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);

        }


        [InlineData(1, "USD", "ADD")]

        [Theory]
        public async void GetExchangeRatesInvalidCurrencyCode(decimal amount, string fromCurrencyCode, string toCurrencyCode)
        {
            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();

            var supportedCurrencyResponse = "{\"AUD\":\"Australian Dollar\",\"BGN\":\"Bulgarian Lev\",\"USD\":\"United States Dollar\"}";
            HttpResponseMessage SupportedhttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(supportedCurrencyResponse, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<JsonObject>(It.IsAny<string>())).ReturnsAsync(SupportedhttpResponseMessage);

            var response = "{\"amount\":1.0,\"base\":\"USD\",\"date\":\"2024-09-13\",\"rates\":{\"INR\":83.92}}";
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<CurrencyExchange>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetExchangeRates(amount, fromCurrencyCode, toCurrencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }



        [InlineData(1, "USD", "AUD")]

        [Theory]
        public async void GetExchangeRatesValidDataWithError(decimal amount, string fromCurrencyCode, string toCurrencyCode)
        {
            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();


            var supportedCurrencyResponse = "{\"AUD\":\"Australian Dollar\",\"BGN\":\"Bulgarian Lev\",\"USD\":\"United States Dollar\"}";
            HttpResponseMessage SupportedhttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(supportedCurrencyResponse, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<JsonObject>(It.IsAny<string>())).ReturnsAsync(SupportedhttpResponseMessage);


            var response = "Content Error";
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<CurrencyExchange>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetExchangeRates(amount, fromCurrencyCode, toCurrencyCode);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }

        [InlineData("01/01/2023", " ", "INR",1,1)]
        [InlineData("01/01/2053", "30/01/2023", "INR", 1, 1)]
        [InlineData("", "30/01/2023", "INR", 1, 1)]
        [InlineData("", "30/01/2023", "INR1", 1, 1)]
        [InlineData("01--01/2023", "30/01/2023", "INR1", 1, 1)]
        [InlineData("01/01/2023", "30//01/2023", "INR1", 1, 1)]
        [InlineData("15/02/2023", "13/02/2023", "INR1", 1, 1)]
        [Theory]
        public async void GetExchangeRateForSpecificPeriodInvalidData(string referenceDateFrom, string referenceDateTo, string currency, int page, int pageSize)
        {
            var controller = new CurrencyController(new CurrencyService());
            var result = await controller.GetExchangeRateForSpecificPeriod(referenceDateFrom, referenceDateTo, currency,page,pageSize);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
        }

        [InlineData("01/01/2023", "30/01/2023", "USD", 1, 1)]
        [Theory]
        public async void GetExchangeRateForSpecificPeriodValidData(string referenceDateFrom, string referenceDateTo, string currency, int page, int pageSize)
        {
          
            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();

            var supportedCurrencyResponse = "{\"AUD\":\"Australian Dollar\",\"BGN\":\"Bulgarian Lev\",\"USD\":\"United States Dollar\"}";
            HttpResponseMessage SupportedhttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(supportedCurrencyResponse, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<JsonObject>(It.IsAny<string>())).ReturnsAsync(SupportedhttpResponseMessage);


            var response = "{ \"amount\": 1.0,\"base\": \"USD\", \"start_date\": \"2023-01-02\", \"end_date\": \"2023-06-01\", \"rates\": {  \"2023-01-01\": { \"AUD\": 1.4695 }, \"2023-01-02\": {\"AUD\": 1.4695}, \"2023-01-03\": { \"AUD\": 1.4695 }, \"2023-01-042\": { \"AUD\": 1.4695 } }}";
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<ExchangeBaseApiResponse>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetExchangeRateForSpecificPeriod(referenceDateFrom, referenceDateTo, currency, page, pageSize);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
            var pagingValue = (ExchangeBaseApiResponse)((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
            Assert.NotNull(pagingValue);
            Assert.Equal(1, pagingValue.Rates.Count());
        }

        [InlineData("01/01/2023", "30/01/2023", "USD", 1, 2)]
        [Theory]
        public async void GetExchangeRateForSpecificPeriodValidDataPagingTest(string referenceDateFrom, string referenceDateTo, string currency, int page, int pageSize)
        {

            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();

            var supportedCurrencyResponse = "{\"AUD\":\"Australian Dollar\",\"BGN\":\"Bulgarian Lev\",\"USD\":\"United States Dollar\"}";
            HttpResponseMessage SupportedhttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(supportedCurrencyResponse, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<JsonObject>(It.IsAny<string>())).ReturnsAsync(SupportedhttpResponseMessage);


            var response = "{ \"amount\": 1.0,\"base\": \"USD\", \"start_date\": \"2023-01-02\", \"end_date\": \"2023-06-01\", \"rates\": {  \"2023-01-01\": { \"AUD\": 1.4695 }, \"2023-01-02\": {\"AUD\": 1.4695}, \"2023-01-03\": { \"AUD\": 1.4695 }, \"2023-01-042\": { \"AUD\": 1.4695 } }}";

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };


            frankfurterClientmock.Setup(x => x.GetAsync<ExchangeBaseApiResponse>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);
            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetExchangeRateForSpecificPeriod(referenceDateFrom, referenceDateTo, currency, page, pageSize);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
            var pagingValue = (ExchangeBaseApiResponse)((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
            Assert.NotNull(pagingValue);
            Assert.Equal(2, pagingValue.Rates.Count());
        }

        [InlineData("01/01/2023", "30/01/2023", "USD", 30, 30)]
        [Theory]
        public async void GetExchangeRateForSpecificPeriodValidDataPagingInvalidTest(string referenceDateFrom, string referenceDateTo, string currency, int page, int pageSize)
        {

            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();


            var supportedCurrencyResponse = "{\"AUD\":\"Australian Dollar\",\"BGN\":\"Bulgarian Lev\",\"USD\":\"United States Dollar\"}";
            HttpResponseMessage SupportedhttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(supportedCurrencyResponse, Encoding.UTF8, "application/json")
            };
            frankfurterClientmock.Setup(x => x.GetAsync<JsonObject>(It.IsAny<string>())).ReturnsAsync(SupportedhttpResponseMessage);


            var response = "{ \"amount\": 1.0,\"base\": \"USD\", \"start_date\": \"2023-01-02\", \"end_date\": \"2023-06-01\", \"rates\": {  \"2023-01-01\": { \"AUD\": 1.4695 }, \"2023-01-02\": {\"AUD\": 1.4695}, \"2023-01-03\": { \"AUD\": 1.4695 }, \"2023-01-042\": { \"AUD\": 1.4695 } }}";

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };


            frankfurterClientmock.Setup(x => x.GetAsync<ExchangeBaseApiResponse>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);
            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetExchangeRateForSpecificPeriod(referenceDateFrom, referenceDateTo, currency, page, pageSize);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
            var pagingValue = (ExchangeBaseApiResponse)((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
            Assert.NotNull(pagingValue);
            Assert.Equal(4, pagingValue.Rates.Count());
        }
        [InlineData("01/01/2023", "30/01/2023", "INR", 30, 30)]
        [InlineData("01/01/2023", "30/01/2023", "inr", 30, 30)]
        [Theory]
        public async void GetExchangeRateForSpecificPeriodInvalidTest(string referenceDateFrom, string referenceDateTo, string currency, int page, int pageSize)
        {

            Mock<IFrankfurterClient> frankfurterClientmock = new Mock<IFrankfurterClient>();
            var response = "Invalid Content";

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            {

                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };


            frankfurterClientmock.Setup(x => x.GetAsync<ExchangeBaseApiResponse>(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);
            var currencyService = new CurrencyService();
            currencyService.frankfurterClient = frankfurterClientmock.Object;
            var controller = new CurrencyController(currencyService);
            var result = await controller.GetExchangeRateForSpecificPeriod(referenceDateFrom, referenceDateTo, currency, page, pageSize);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode);
           
        }
    }
}