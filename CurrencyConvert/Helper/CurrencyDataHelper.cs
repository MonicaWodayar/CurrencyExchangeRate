using System.Globalization;
using System.Text;

namespace CurrencyConvert.Helper
{
    public class CurrencyDataHelper
    {
        public static string DateIntervalToString(DateTime startDate, DateTime? fromDate)
        {
            var dateInterval = new StringBuilder(startDate.ToString("yyyy-MM-dd"));

            dateInterval.Append("..");

            if (fromDate != null)
            {
                dateInterval.Append(fromDate?.ToString("yyyy-MM-dd"));
            }

            return dateInterval.ToString();
        }

        public static bool IsCurrencyExcluded(string currencyList, string currencyCode)
        {           
            var currencies = currencyList.Split(',');         
            return currencies.Contains(currencyCode, StringComparer.OrdinalIgnoreCase);
        }
        public static bool TryParseDate(string dateString, out DateTime dateTime)
        {          
            string[] formats = {  "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy" };            
            return DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }
    }
}
