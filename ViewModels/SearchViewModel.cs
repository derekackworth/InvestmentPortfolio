/* 
    Author: Derek Ackworth
    Purpose: Modify model variables, modify view variables, handle commands, and notify the view of changes made
*/

using InvestmentPortfolio.Bases;
using InvestmentPortfolio.Facades;
using InvestmentPortfolio.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace InvestmentPortfolio.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private int _column;
        public int Column
        {
            get
            {
                return _column;
            }
            set
            {
                _column = value;
                OnPropertyChanged(nameof(Column));
            }
        }

        private int _columnSpan;
        public int ColumnSpan
        {
            get
            {
                return _columnSpan;
            }
            set
            {
                _columnSpan = value;
                OnPropertyChanged(nameof(ColumnSpan));
            }
        }

        private string _keyword;
        public string Keyword
        {
            get
            {
                return _keyword;
            }

            set
            {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
            }
        }

        private List<SearchResult> _searchResults;
        public List<SearchResult> SearchResults
        {
            get
            {
                return _searchResults;
            }
            set
            {
                _searchResults = value;
                OnPropertyChanged(nameof(SearchResults));
            }
        }

        private SearchResult _selectedResult;
        public SearchResult SelectedResult
        {
            get
            {
                return _selectedResult;
            }
            set
            {
                _selectedResult = value;
                OnPropertyChanged(nameof(SelectedResult));
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        private string _amount;
        public string Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                if (!value.Contains('-') && !value.Contains(',') && (double.TryParse(value, out _) || value == ""))
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        private string _price;
        public string Price
        {
            get
            {
                return _price;
            }
            set
            {

                if (!value.Contains('-') && !value.Contains(',') && (double.TryParse(value, out _) || value == ""))
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        public CommandBase SearchCommand { get; set; }
        private void Search(object obj)
        {
            SearchResults = AlphaVantageAPIFacade.Search(Keyword);
        }

        public CommandBase AddInvestmentCommand { get; set; }
        private void AddInvestment(object obj)
        {
            string message = "";
            double amount = 0;
            double price = 0;

            if (string.IsNullOrWhiteSpace(Amount))
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    message += "\n";
                }

                message += "Amount must be > 0.";
            }
            else
            {
                amount = double.Parse(Amount);

                if (amount <= 0)
                {
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        message += "\n";
                    }

                    message += "Amount must be > 0.";
                }
            }

            if (string.IsNullOrWhiteSpace(Price))
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    message += "\n";
                }

                message += "Price paid must be >= 0.";
            }
            else
            {
                price = double.Parse(Price);

                if (price < 0)
                {
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        message += "\n";
                    }

                    message += "Price paid must be >= 0.";
                }
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message, "Investment Add Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Investment investment = new Investment(Date, SelectedResult.Name, SelectedResult.Symbol, SelectedResult.Type, SelectedResult.Currency, amount, price);
                message = DatabaseFacade.CreateInvestment(User.UserId, investment);

                if (!message.Contains("Exception"))
                {
                    investment.InvestmentId = Convert.ToUInt32(message);
                    investment.CalculateNetGain();
                    User.Investments.Add(investment);
                    MessageBox.Show("Investment added", "Successfully Added Investment", MessageBoxButton.OK, MessageBoxImage.Information);
                    Date = DateTime.Now.Date;
                    Amount = "";
                    Price = "";
                    SelectedResult = default;
                }
                else
                {
                    MessageBox.Show(message, "Investment Add Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return;
        }

        public SearchViewModel()
        {
            Title += " (" + User.Username + ") | Search";
            Column = 1;
            ColumnSpan = 1;
            Keyword = "";
            Date = DateTime.Now.Date;
            Amount = "";
            Price = "";
            SearchResults = new List<SearchResult>();
            SearchCommand = new CommandBase(Search);
            AddInvestmentCommand = new CommandBase(AddInvestment);
        }
}
}
