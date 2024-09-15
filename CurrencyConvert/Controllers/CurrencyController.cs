using Microsoft.AspNetCore.Mvc;
using CurrencyConvert.Interface;
using CurrencyConvert.Helper;


namespace CurrencyConvert.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private ICurrencyService currencyService;
        public CurrencyController(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }


        [HttpGet("GetExchangeRate")]
        public async Task<IActionResult> GetAllAvailableCurrenciesAsync(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3)
            {
                var errorResponse = new
                {
                    Message = "Required Query parameters not sent or Invalid"
                };

                return new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            try
            {
                var isCurrencySupported = await currencyService.GetSupportedCurrenciesAsync(currencyCode);
                if(!isCurrencySupported)
                {
                    var errorResponse = new
                    {
                        Message = "Currency Code Not Supported"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var result = await currencyService.GetExchangeRateForBaseCurrency(currencyCode);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

            }

        }

        [HttpGet("GetSpecificExchangeRate")]

        public async Task<IActionResult> GetExchangeRates(decimal amount, string fromCurrencyCode, string toCurrencyCode)
        {
            if (string.IsNullOrWhiteSpace(fromCurrencyCode) || string.IsNullOrWhiteSpace(toCurrencyCode) || fromCurrencyCode.Length != 3 || toCurrencyCode.Length != 3)
            {
                var errorResponse = new
                {
                    Message = "Required Query parameters not sent or Invalid"
                };

                return new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            if (amount <= 0)
            {
                var errorResponse = new
                {
                    Message = "Amount cannot be less than or equal to 0"
                };

                return new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            try
            {
                if (fromCurrencyCode == toCurrencyCode)
                {
                    var errorResponse = new
                    {
                        Message = "From and To currency code is same"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var excludedCurrency = ConfigurationHelper.ExcludedCurrencyCode();
                bool isFromExcluded = CurrencyDataHelper.IsCurrencyExcluded(excludedCurrency, fromCurrencyCode);
                bool isToExcluded = CurrencyDataHelper.IsCurrencyExcluded(excludedCurrency, toCurrencyCode);
                if (isFromExcluded || isToExcluded)
                {
                    var errorResponse = new
                    {
                        Message = "Unsupported Currency Code"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var isFromCurrencySupported = await currencyService.GetSupportedCurrenciesAsync(fromCurrencyCode);
                var isToCurrencySupported = await currencyService.GetSupportedCurrenciesAsync(toCurrencyCode);
                if (!isFromCurrencySupported ||!isToCurrencySupported)
                {
                    var errorResponse = new
                    {
                        Message = "Currency Code Not Supported"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var result = await currencyService.CurrencyConvertAsync(amount, fromCurrencyCode, toCurrencyCode);
                return Ok(result);

            }
            catch (Exception ex)
            {

                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

        }

        [HttpGet("GetExchangeRateForSpecificPeriod")]

        public async Task<IActionResult> GetExchangeRateForSpecificPeriod(string referenceDateFrom, string referenceDateTo, string currency, int page, int pageSize)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(referenceDateFrom) || string.IsNullOrWhiteSpace(referenceDateTo) || string.IsNullOrEmpty(currency)
                    || currency.Length != 3)
                {
                    var errorResponse = new
                    {
                        Message = "Required Query parameters not sent or Invalid"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                DateTime startDate;
                DateTime endDate;
                if (!CurrencyDataHelper.TryParseDate(referenceDateFrom, out startDate))
                {

                    var errorResponse = new
                    {
                        Message = "Invalid Date From"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                if (!CurrencyDataHelper.TryParseDate(referenceDateTo, out endDate))
                {
                    var errorResponse = new
                    {
                        Message = "Invalid Date To"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                if(startDate>DateTime.Today)
                {
                    var errorResponse = new
                    {
                        Message = "Date From cannot be future date"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                if(endDate>DateTime.Today)
                {
                    endDate= DateTime.Today;
                }

                if(endDate<startDate)
                {
                    var errorResponse = new
                    {
                        Message = "Date To cannot be lesser than Date From"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var isCurrencySupported = await currencyService.GetSupportedCurrenciesAsync(currency);
                if (!isCurrencySupported)
                {
                    var errorResponse = new
                    {
                        Message = "Currency Code Not Supported"
                    };

                    return new ObjectResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 1;
                var result = await currencyService.CurrencyConvertByDateAsync(startDate, endDate, currency, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

            }
        }
    }
}
