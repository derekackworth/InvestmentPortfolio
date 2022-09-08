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
    public class DashboardViewModel : ViewModelBase
    {
        private readonly NavigationBase _navigation;

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

        private List<Investment> _investments;
        public List<Investment> Investments
        {
            get
            {
                return _investments;
            }
            set
            {
                _investments = value;
                OnPropertyChanged(nameof(Investments));
            }
        }

        private Investment _selectedInvestment;
        public Investment SelectedInvestment
        {
            get
            {
                return _selectedInvestment;
            }
            set
            {
                _selectedInvestment = value;

                if (_selectedInvestment == default)
                {
                    Date = default;
                    Amount = "";
                    Price = "";
                }
                else
                {
                    Date = SelectedInvestment.Date.Date;
                    Amount = SelectedInvestment.Amount.ToString();
                    Price = SelectedInvestment.Price.ToString();
                }

                OnPropertyChanged(nameof(SelectedInvestment));
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

        public CommandBase AddInvestmentCommand { get; set; }
        private void AddInvestment(object obj)
        {
            _navigation.CurrentViewModel = new SearchViewModel();
        }

        public CommandBase UpdateInvestmentCommand { get; set; }
        private void UpdateInvestment(object obj)
        {
            string message = DatabaseFacade.UpdateInvestment(SelectedInvestment, Date, Amount, Price);

            if (!message.Contains("Exception"))
            {
                MessageBox.Show(message, "Successfully Updated Investment", MessageBoxButton.OK, MessageBoxImage.Information);
                int edit = User.Investments.FindIndex(investment => investment.InvestmentId == SelectedInvestment.InvestmentId);
                User.Investments[edit].Date = Date;
                User.Investments[edit].Amount = double.Parse(Amount);
                User.Investments[edit].Price = double.Parse(Price);
                User.Investments[edit].CalculateNetGain();
                Investments = default;
                Investments = User.Investments;
                SelectedInvestment = default;
            }
            else
            {
                MessageBox.Show(message, "Investment Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public CommandBase DeleteInvestmentCommand { get; set; }
        private void DeleteInvestment(object obj)
        {
            string message = DatabaseFacade.DeleteInvestment(SelectedInvestment);

            if (!message.Contains("Exception"))
            {
                MessageBox.Show(message, "Successfully Deleted Investment", MessageBoxButton.OK, MessageBoxImage.Information);
                User.Investments.Remove(SelectedInvestment);
                Investments = User.Investments;
                SelectedInvestment = default;
            }
            else
            {
                MessageBox.Show(message, "Investment Delete Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DashboardViewModel(NavigationBase navigation)
        {
            _navigation = navigation;
            Title += " (" + User.Username + ") | Dashboard";
            Column = 1;
            ColumnSpan = 1;
            Investments = User.Investments;
            AddInvestmentCommand = new CommandBase(AddInvestment);
            UpdateInvestmentCommand = new CommandBase(UpdateInvestment);
            DeleteInvestmentCommand = new CommandBase(DeleteInvestment);
        }
    }
}
