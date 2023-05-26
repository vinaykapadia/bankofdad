using BankOfDad.Client;

namespace BankOfDad.Gui.Views;

public partial class HomePage : ContentPage
{
    private readonly BankOfDadClient _client;

    public HomePage(BankOfDadClient client)
    {
        _client = client;

        InitializeComponent();
	}
    
    protected override void OnAppearing()
    {
        var thread = new Thread(async () => { await UpdateAccount(); });
        thread.Start();

        base.OnAppearing();
    }

    private async Task UpdateAccount()
    {
        var token = SecureStorage.GetAsync("token").Result;
        _client.SetToken(token);

        var account = await _client.GetAccount();

        if (account == null)
        {
            SecureStorage.Remove("token");
            Dispatcher.Dispatch(async () => { await Shell.Current.GoToAsync("///login"); });
            return;
        }

        Dispatcher.Dispatch(() =>
        {
            LblName.Text = account.Name;
            LblBalance.Text = account.Balance.HasValue ? $"{account.Balance:C}" : "$∞";
            LstCollection.ItemsSource = account.Transactions;
        });
    }

    private void BtnLogout_Clicked(object sender, EventArgs args)
    {
        SecureStorage.Remove("token");
        Dispatcher.Dispatch(async () => { await Shell.Current.GoToAsync("///login"); });
    }
}