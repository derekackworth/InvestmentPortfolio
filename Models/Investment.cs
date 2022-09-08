/* 
    Author: Derek Ackworth
    Purpose: Store investment information
*/

using InvestmentPortfolio.Facades;
using System;

namespace InvestmentPortfolio.Models
{
    public class Investment
    {
        public uint InvestmentId { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public string Type { get; private set; }
        public string Currency { get; private set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public double Worth { get; private set; }
        public double Today { get; private set; }
        public double Last7Days { get; private set; }
        public double Last30Days { get; private set; }
        public double AllTime { get; private set; }

        public Investment(uint investmentId, DateTime date, string name, string symbol, string type, string currency, double amount, double price)
        {
            InvestmentId = investmentId;
            Date = date;
            Name = name;
            Symbol = symbol;
            Type = type;
            Currency = currency;
            Amount = amount;
            Price = price;
            CalculateNetGain();
        }

        public Investment(DateTime date, string name, string symbol, string type, string currency, double amount, double price)
        {
            Date = date;
            Name = name;
            Symbol = symbol;
            Type = type;
            Currency = currency;
            Amount = amount;
            Price = price;
        }

        public void CalculateNetGain()
        {
            int day = 1;
            int daysAgo = (DateTime.Now.Date - Date).Days;
            double priceToday = 0;

            if (Type == "Crypto")
            {
                foreach (dynamic date in AlphaVantageAPIFacade.CryptoQuery(Symbol))
                {
                    foreach (dynamic dateData in date)
                    {
                        if (day == 1)
                        {
                            priceToday = Math.Round(Convert.ToDouble(dateData["4a. close (USD)"].ToString()), 2);
                            AllTime = Math.Round(priceToday * Amount - Price, 2);
                            Today = Math.Round((priceToday - Convert.ToDouble(dateData["1a. open (USD)"].ToString())) * Amount, 2);
                            Worth = Price + AllTime;
                        }
                        else if (day == 7)
                        {
                            Last7Days = Math.Round((priceToday - Convert.ToDouble(dateData["1a. open (USD)"].ToString())) * Amount, 2);
                        }
                        else if (day == 30)
                        {
                            Last30Days = Math.Round((priceToday - Convert.ToDouble(dateData["1a. open (USD)"].ToString())) * Amount, 2);
                            break;
                        }
                        else if (day == daysAgo)
                        {
                            if (Last7Days == default)
                            {
                                Last7Days = Math.Round((priceToday - Convert.ToDouble(dateData["1a. open (USD)"].ToString())) * Amount, 2);
                            }
                            
                            if (Last30Days == default)
                            {
                                Last30Days = Math.Round((priceToday - Convert.ToDouble(dateData["1a. open (USD)"].ToString())) * Amount, 2);
                            }

                            break;
                        }

                        day++;
                    }
                }
            }
            else if (Type == "Stock")
            {
                if (!AlphaVantageAPIFacade.ToUsd.ContainsKey(Currency))
                {
                    AlphaVantageAPIFacade.ToUsd.Add(Currency, Convert.ToDouble(AlphaVantageAPIFacade.CurrencyQuery(Currency)["5. Exchange Rate"].ToString()));
                }

                foreach (dynamic date in AlphaVantageAPIFacade.StockQuery(Symbol))
                {
                    foreach (dynamic dateData in date)
                    {
                        if (day == 1)
                        {
                            priceToday = Math.Round(Convert.ToDouble(dateData["4. close"].ToString()), 2);
                            AllTime = Math.Round(priceToday * AlphaVantageAPIFacade.ToUsd[Currency] * Amount - Price, 2);
                            Today = Math.Round((priceToday - Convert.ToDouble(dateData["1. open"].ToString())) * AlphaVantageAPIFacade.ToUsd[Currency] * Amount, 2);
                            Worth = Price + AllTime;
                        }
                        else if (day == 7)
                        {
                            Last7Days = Math.Round((priceToday - Convert.ToDouble(dateData["1. open"].ToString())) * AlphaVantageAPIFacade.ToUsd[Currency] * Amount, 2);
                        }
                        else if (day == 30)
                        {
                            Last30Days = Math.Round((priceToday - Convert.ToDouble(dateData["1. open"].ToString())) * AlphaVantageAPIFacade.ToUsd[Currency] * Amount, 2);
                            break;
                        }
                        else if (day == daysAgo)
                        {
                            if (Last7Days == default)
                            {
                                Last7Days = Math.Round((priceToday - Convert.ToDouble(dateData["1. open"].ToString())) * AlphaVantageAPIFacade.ToUsd[Currency] * Amount, 2);
                            }

                            if (Last30Days == default)
                            {
                                Last30Days = Math.Round((priceToday - Convert.ToDouble(dateData["1. open"].ToString())) * AlphaVantageAPIFacade.ToUsd[Currency] * Amount, 2);
                            }

                            break;
                        }

                        day++;
                    }
                }
            }
        }
    }
}
