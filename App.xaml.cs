/* 
    Author: Derek Ackworth
    Purpose: Set culture formatting and main window view
*/

using InvestmentPortfolio.Bases;
using InvestmentPortfolio.ViewModels;
using InvestmentPortfolio.Views;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace InvestmentPortfolio
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag)));
            NavigationBase NavigationStore = new NavigationBase();
            NavigationStore.CurrentViewModel = new LogInViewModel(NavigationStore);

            MainWindow = new MainView()
            {
                DataContext = new MainViewModel(NavigationStore)
            };

            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
