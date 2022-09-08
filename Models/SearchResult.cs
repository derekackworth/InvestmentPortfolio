/* 
    Author: Derek Ackworth
    Purpose: Store search result information
*/

namespace InvestmentPortfolio.Models
{
    public class SearchResult
    {
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public string Type { get; private set; }
        public string Currency { get; private set; }
        public double Price { get; private set; }
        public double Today { get; private set; }
        public double Last7Days { get; private set; }
        public double Last30Days { get; private set; }

        public SearchResult(string name, string symbol, string type, string currency, double price, double today, double last7Days, double last30Days)
        {
            Name = name;
            Symbol = symbol;
            Type = type;
            Currency = currency;
            Price = price;
            Today = today;
            Last7Days = last7Days;
            Last30Days = last30Days;
        }
    }
}
