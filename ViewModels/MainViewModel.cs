/* 
    Author: Derek Ackworth
    Purpose: Modify view variables and notify the view of changes made
*/

using InvestmentPortfolio.Bases;

namespace InvestmentPortfolio.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationBase _navigation;

        private SidebarViewModel _sidebarViewModel;
        public SidebarViewModel SidebarViewModel
        {
            get
            {
                return _sidebarViewModel;
            }
            set
            {
                _sidebarViewModel = value;
                OnPropertyChanged(nameof(SidebarViewModel));
            }
        }

        public ViewModelBase CurrentViewModel => _navigation.CurrentViewModel;
        private void OnCurrentViewModelChanged()
        {
            if (CurrentViewModel is LogInViewModel && SidebarViewModel != default)
            {
                SidebarViewModel = default;
            }
            else
            {
                if (SidebarViewModel == default)
                {
                    SidebarViewModel = new SidebarViewModel(_navigation);
                }

                if (CurrentViewModel is DashboardViewModel)
                {
                    SidebarViewModel.DashboardEnabled = false;
                    SidebarViewModel.SearchEnabled = true;
                }
                else if (CurrentViewModel is SearchViewModel)
                {
                    SidebarViewModel.DashboardEnabled = true;
                    SidebarViewModel.SearchEnabled = false;
                }
            }

            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public MainViewModel(NavigationBase navigation)
        {
            _navigation = navigation;
            _navigation.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }
    }
}
