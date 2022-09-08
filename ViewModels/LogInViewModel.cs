/* 
    Author: Derek Ackworth
    Purpose: Modify model variables, modify view variables, handle commands, and notify the view of changes made
*/

using InvestmentPortfolio.Bases;
using InvestmentPortfolio.Facades;
using InvestmentPortfolio.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace InvestmentPortfolio.ViewModels
{
    public class LogInViewModel : ViewModelBase
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

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                if (Regex.IsMatch(value, @"^[a-zA-Z0-9]+$") || value == "")
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));

                }
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = Regex.Replace(value, @"\s+", "");
                OnPropertyChanged(nameof(Password));
            }
        }

        public CommandBase LogInCommand { get; set; }
        private void LogIn(object obj)
        {
            string message = "";

            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 8 || Username.Length > 20)
            {
                message += "Username must be 8-20 characters.";
            }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8 || Password.Length > 100)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    message += "\n";
                }

                message += "Password must be 8-100 characters.";
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message, "Log In Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                message = DatabaseFacade.ReadUser(Username, Password);

                if (!message.Contains("Exception") && !message.Contains("Invalid"))
                {
                    User.LogIn(Convert.ToUInt32(message), Username);
                    _navigation.CurrentViewModel = new DashboardViewModel(_navigation);
                }
                else
                {
                    MessageBox.Show(message, "Log In Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return;
        }

        public CommandBase SignUpCommand { get; set; }
        private void SignUp(object obj)
        {
            string message = "";

            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 6 || Username.Length > 20)
            {
                message += "Username must be 6-20 characters.";
            }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8 || Password.Length > 100)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    message += "\n";
                }

                message += "Password must be 8-100 characters.";
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message, "Sign Up Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                message = DatabaseFacade.CreateUser(Username, Password);

                if (!message.Contains("Exception"))
                {
                    MessageBox.Show(message, "Successfully Signed Up", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(message, "Sign Up Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return;
        }

        public LogInViewModel(NavigationBase navigation)
        {
            User.LogOut();
            _navigation = navigation;
            Title += " | Log In";
            Column = 0;
            ColumnSpan = 2;
            Username = "";
            Password = "";
            LogInCommand = new CommandBase(LogIn);
            SignUpCommand = new CommandBase(SignUp);
        }
    }
}
