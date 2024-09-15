Currency Converter 

Below are the API's inmplemented 

--GetExchangeRate
Gets the exchange rate for the sent base currency
Parameter Expected:
currencyCode in string

--GetSpecificExchangeRate
Gets the exchange rate for the passed amount and currency from and currency to
Parameter Expected:
1.amount in decimal,
2.fromCurrencyCode in string 
3.toCurrencyCode in string

--GetExchangeRateForSpecificPeriod
Gets the exchange rate for specific period also paging value is expected here

Parameter Expected:
1.referenceDateFrom in string (supported date format {"yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy" })
2.referenceDateTo in string (supported date format {"yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy" })
3.currency in string
4.page in int (which page to be viewed)
5.pageSize in int ( how many pages to be viewed)

-------------------------------------------------------------------------------------------------------------------------------

Unit test written in Xunit: MOQ framework is installed 
Nuget package installed: Flurl, Polly, Newtonsoft.json

Note:
Base url and excluded currency code is placed in appsettings.json

 "BaseURl": "https://api.frankfurter.app/",
 "ExcludedCurrencyCode": "TRY,PLN,THB,MXN"