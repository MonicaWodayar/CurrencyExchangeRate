namespace CurrencyConvert.Helper
{
    public static class ConfigurationHelper
    {

        public static IConfiguration Configuration { get; set; }


        public static string GetBaseURL()
        {
            string? siteTitle = Configuration["AppSettings:BaseURl"];
            return siteTitle == null ? string.Empty : siteTitle;

        }

        public static string ExcludedCurrencyCode()
        {
            string? excludedCurrencyCode = Configuration["AppSettings:ExcludedCurrencyCode"];
            return excludedCurrencyCode == null ? string.Empty : excludedCurrencyCode;

        }

    }
}
