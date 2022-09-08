/* 
    Author: Derek Ackworth
    Purpose: More easily query the API
*/

using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using InvestmentPortfolio.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InvestmentPortfolio.Facades
{
    public static class AlphaVantageAPIFacade
    {
        private static readonly string _apiKey = ConfigurationManager.AppSettings["APIKey"];

        private static readonly Dictionary<string, string> _cryptos = new Dictionary<string, string>()
        {
            {"AAVE", "Aave"}, {"ADA", "Cardano"}, {"ADX", "AdEx"}, {"AION", "Aion"}, {"ALGO", "Algorand"}, {"AMP", "Synereo"}, {"ANC", "Anoncoin"}, {"ANT", "Aragon"},
            {"ARDR", "Ardor"}, {"ATM", "ATMChain"}, {"ATOM", "Cosmos"}, {"AVAX", "Avalanche"}, {"BAND", "Band Protocol"}, {"BAT", "Basic-Attention-Token"}, {"BCC", "BitConnect"},
            {"BCH", "Bitcoin-Cash"}, {"BLZ", "Bluzelle"}, {"BNB", "Binance-Coin"}, {"BNT", "Bancor-Network-Token"}, {"BTC", "Bitcoin"}, {"BTG", "Bitcoin-Gold"},
            {"BTS", "BitShares"}, {"BTT", "BitTorrent"}, {"BUSD", "Binance-USD"}, {"CAKE", "PancakeSwap"}, {"COMP", "Compound"}, {"CTXC", "Cortex"}, {"CVC", "Civic"},
            {"DAI", "Dai"}, {"DAR", "Darcrus"}, {"DASH", "Dash"}, {"DATA", "DATAcoin"}, {"DCR", "Decred"}, {"DENT", "Dent"}, {"DGB", "DigiByte"}, {"DNT", "district0x"},
            {"DOGE", "DogeCoin"}, {"DOT", "Polkadot"}, {"EGLD", "Elrond"}, {"ELF", "aelf"}, {"ENJ", "Enjin-Coin"}, {"EOS", "EOS"}, {"ETC", "Ethereum-Classic"},
            {"ETH", "Ethereum"}, {"FIL", "Filecoin"}, {"FTT", "FTX Token"}, {"FUN", "FunFair"}, {"GNO", "Gnosis-Token"}, {"GRT", "Graph"}, {"GTC", "Game"}, {"GTO", "Gifto"},
            {"GXS", "GXShares"}, {"HBAR", "Hedera"}, {"ICX", "ICON"}, {"IOST", "IOStoken"}, {"IOTA", "IOTA"}, {"IOTX", "IoTeX"}, {"KLAY", "Klaytn"}, {"KMD", "Komodo"},
            {"KNC", "Kyber-Network"}, {"KSM", "Kusama"}, {"LEND", "EthLend"}, {"LINK", "ChainLink"}, {"LRC", "Loopring"}, {"LSK", "Lisk"}, {"LTC", "Litecoin"}, {"LUNA", "Terra"},
            {"MANA", "Decentraland"}, {"MATIC", "Polygon"}, {"MCO", "Monaco"}, {"MITH", "Mithril"}, {"MKR", "Maker"}, {"MLN", "Melon"}, {"NANO", "Nano"}, {"NBT", "NuBits"},
            {"NEO", "NEO"}, {"NMR", "Numeraire"}, {"NPXS", "Pundi-X-Token"}, {"NULS", "Nuls"}, {"OMG", "OmiseGo"}, {"ONT", "Ontology"}, {"ORN", "Orion-Protocol"},
            {"POLY", "Polymath"}, {"POWR", "Power-Ledger"}, {"QTUM", "Qtum"}, {"QUICK", "Quickswap"}, {"REP", "Augur"}, {"REQ", "Request-Network"}, {"RLC", "RLC-Token"},
            {"RUNE", "THORChain"}, {"SC", "Siacoin"}, {"SHIB", "SHIBA-INU"}, {"SOL", "Solana"}, {"SPC", "SpaceChain"}, {"SNX", "Synthetix-Network-Token"}, {"STEEM", "Steem"},
            {"STORJ", "Storj"}, {"STORM", "Storm"}, {"STRAT", "Stratis"}, {"STX", "Stox"}, {"SYS", "SysCoin"}, {"THETA", "Theta-Token"}, {"TRIBE", "Tribe"}, {"TRX", "Tronix"},
            {"TUSD", "TrueUSD"}, {"UNI", "Uniswap"}, {"UST", "TerraUSD"}, {"UTK", "UTrust"}, {"VEN", "VeChain"}, {"VET", "VeChain"}, {"WAN", "Wanchain"}, {"WAVES", "Waves"},
            {"WTC", "Walton"}, {"XEM", "NEM"}, {"XLM", "Stellar"}, {"XMR", "Monero"}, {"XRP", "Ripple"}, {"XTZ", "Tezos"}, {"XVG", "Verge"}, {"XZC", "ZCoin"}, {"ZEC", "Zcash"},
            {"ZEN", "ZenCash"}, {"ZIL", "Zilliqa"}, {"ZRX", "0x"}
        };

        public static Dictionary<string, double> ToUsd = new Dictionary<string, double>() { {"USD", 1.00} };

        public static List<SearchResult> Search(string keyword)
        {
            List<SearchResult> searchResults = new List<SearchResult>();

            foreach (KeyValuePair<string, string> cryptoMatch in CryptoSearchQuery(keyword))
            {
                string name = cryptoMatch.Value;
                string symbol = cryptoMatch.Key;
                string type = "Crypto";
                string currency = "USD";
                double price = 0;
                double today = 0;
                double last7Days = 0;
                double last30Days = 0;
                int day = 1;

                foreach (dynamic date in CryptoQuery(symbol))
                {
                    foreach (dynamic dateData in date)
                    {
                        if (day == 1)
                        {
                            price = Math.Round(Convert.ToDouble(dateData["4a. close (USD)"].ToString()), 2);
                            today = Math.Round(price - Convert.ToDouble(dateData["1a. open (USD)"].ToString()), 2);
                        }
                        else if (day == 7)
                        {
                            last7Days = Math.Round(price - Convert.ToDouble(dateData["1a. open (USD)"].ToString()), 2);
                        }
                        else if (day == 30)
                        {
                            last30Days = Math.Round(price - Convert.ToDouble(dateData["1a. open (USD)"].ToString()), 2);
                            break;
                        }

                        day++;
                    }
                }

                searchResults.Add(new SearchResult(name, symbol, type, currency, price, today, last7Days, last30Days));
            }

            foreach (dynamic stockMatch in StockSearchQuery(keyword))
            {
                string name = stockMatch["2. name"].ToString();
                string symbol = stockMatch["1. symbol"].ToString();
                string type = "Stock";
                string currency = stockMatch["8. currency"].ToString();
                double price = 0;
                double today = 0;
                double last7Days = 0;
                double last30Days = 0;
                int day = 1;

                if (!ToUsd.ContainsKey(currency))
                {
                    ToUsd.Add(currency, Convert.ToDouble(CurrencyQuery(currency)["5. Exchange Rate"].ToString()));
                }

                try
                {
                    foreach (dynamic date in StockQuery(symbol))
                    {
                        foreach (dynamic dateData in date)
                        {
                            if (day == 1)
                            {
                                price = Math.Round(Convert.ToDouble(dateData["4. close"].ToString()) * ToUsd[currency], 2);
                                today = Math.Round((price - Convert.ToDouble(dateData["1. open"].ToString())) * ToUsd[currency], 2);
                            }
                            else if (day == 7)
                            {
                                last7Days = Math.Round((price - Convert.ToDouble(dateData["1. open"].ToString())) * ToUsd[currency], 2);
                            }
                            else if (day == 30)
                            {
                                last30Days = Math.Round((price - Convert.ToDouble(dateData["1. open"].ToString())) * ToUsd[currency], 2);
                                break;
                            }

                            day++;
                        }
                    }
                }
                catch { }

                searchResults.Add(new SearchResult(name, symbol, type, currency, price, today, last7Days, last30Days));
            }

            return searchResults;
        }

        public static dynamic CurrencyQuery(string currency)
        {
            WebClient client = new WebClient();
            string queryURL = "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=" + currency + "&to_currency=USD&apikey=" + _apiKey;
            Uri queryUri = new Uri(queryURL);

            Query:
                dynamic queryData = JsonConvert.DeserializeObject(client.DownloadString(queryUri));

            if (queryData["Note"] != null)
            {
                Task.Delay(61000).Wait();
                goto Query;
            }

            return queryData["Realtime Currency Exchange Rate"];
        }

        public static dynamic CryptoQuery(string symbol)
        {
            WebClient client = new WebClient();
            string queryURL = "https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_DAILY&symbol=" + symbol + "&market=USD&apikey=" + _apiKey;
            Uri queryUri = new Uri(queryURL);

            Query:
                dynamic queryData = JsonConvert.DeserializeObject(client.DownloadString(queryUri));

            if (queryData["Note"] != null)
            {
                Task.Delay(61000).Wait();
                goto Query;
            }

            return queryData["Time Series (Digital Currency Daily)"];
        }

        public static dynamic StockQuery(string symbol)
        {
            WebClient client = new WebClient();
            string queryURL = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=" + symbol + "&apikey=" + _apiKey;
            Uri queryUri = new Uri(queryURL);

            Query:
                dynamic queryData = JsonConvert.DeserializeObject(client.DownloadString(queryUri));

            if (queryData["Note"] != null)
            {
                Task.Delay(61000).Wait();
                goto Query;
            }

            return queryData["Time Series (Daily)"];
        }

        private static Dictionary<string, string> CryptoSearchQuery(string keyword)
        {
            Dictionary<string, string> queryData = new Dictionary<string, string>();
            int i = 1;

            foreach (KeyValuePair<string, string> _crypto in _cryptos)
            {
                if (_crypto.Key.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 || _crypto.Value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    queryData.Add(_crypto.Key, _crypto.Value);
                    i++;
                }

                if (i > 5)
                {
                    break;
                }
            }

            return queryData;
        }

        private static dynamic StockSearchQuery(string keyword)
        {
            WebClient client = new WebClient();
            string queryURL = "https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords=" + keyword + "&apikey=" + _apiKey;
            Uri queryUri = new Uri(queryURL);

            Query:
                dynamic queryData = JsonConvert.DeserializeObject(client.DownloadString(queryUri));


            if (queryData["Note"] != null)
            {
                Task.Delay(61000).Wait();
                goto Query;
            }

            return queryData["bestMatches"];
        }
    }
}
