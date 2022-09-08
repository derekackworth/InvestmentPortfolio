/* 
    Author: Derek Ackworth
    Purpose: Modify view variables, handle commands, and notify the view of changes made
*/

using InvestmentPortfolio.Bases;

namespace InvestmentPortfolio.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        private readonly NavigationBase _navigation;

        private bool _dashboardEnabled;
        public bool DashboardEnabled
        {
            get
            {
                return _dashboardEnabled;
            }
            set
            {
                _dashboardEnabled = value;
                OnPropertyChanged(nameof(DashboardEnabled));
            }
        }

        private bool _searchEnabled;
        public bool SearchEnabled
        {
            get
            {
                return _searchEnabled;
            }
            set
            {
                _searchEnabled = value;
                OnPropertyChanged(nameof(SearchEnabled));
            }
        }

        public CommandBase DashboardCommand { get; set; }
        private void Dashboard(object obj)
        {
            _navigation.CurrentViewModel = new DashboardViewModel(_navigation);
        }

        public CommandBase SearchCommand { get; set; }
        private void Search(object obj)
        {
            _navigation.CurrentViewModel = new SearchViewModel();
        }

        public CommandBase LogOutCommand { get; set; }
        private void LogOut(object obj)
        {
            _navigation.CurrentViewModel = new LogInViewModel(_navigation);
        }

        public SidebarViewModel(NavigationBase navigation)
        {
            _navigation = navigation;
            DashboardCommand = new CommandBase(Dashboard);
            SearchCommand = new CommandBase(Search);
            LogOutCommand = new CommandBase(LogOut);
        }
    }
}
