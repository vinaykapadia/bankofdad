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
        var token = TokenAccess.GetToken().Result;
        _client.SetToken(token);

        var account = await _client.GetAccount();

        if (account == null)
        {
            TokenAccess.RemoveToken();
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
        TokenAccess.RemoveToken();
        Dispatcher.Dispatch(async () => { await Shell.Current.GoToAsync("///login"); });
    }
}