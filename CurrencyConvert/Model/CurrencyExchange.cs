namespace CurrencyConvert.Model
{
    public class CurrencyExchange
    {
        public decimal amount { get; set; }
       
        public string date { get; set; }
        public Dictionary<string, decimal> rates { get; set; }
    }
}
