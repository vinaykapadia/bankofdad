using BankOfDad.Gui.Views;

namespace BankOfDad.Gui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("home", typeof(HomePage));
        }

        protected override bool OnBackButtonPressed()
        {
            Application.Current?.Quit();
            return true;
        }
    }
}