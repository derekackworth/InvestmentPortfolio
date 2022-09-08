/* 
    Author: Derek Ackworth
    Purpose: Hold title information and notifiy the view when properties are changed
*/

using System.ComponentModel;

namespace InvestmentPortfolio.Bases
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public ViewModelBase()
        {
            Title = "Investment Portfolio";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
