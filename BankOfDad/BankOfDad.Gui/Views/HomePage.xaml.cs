using BankOfDad.Client;

namespace BankOfDad.Gui.Views;

public partial class HomePage : ContentPage
{
    private readonly BankOfDadClient _client;
    private readonly ITokenAccess _tokenAccess;

    public HomePage(BankOfDadClient client, ITokenAccess tokenAccess)
    {
        _client = client;
        _tokenAccess = tokenAccess;

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
        var token = _tokenAccess.GetToken().Result;
        _client.SetToken(token);

        var account = await _client.GetAccount();

        if (account == null)
        {
            _tokenAccess.RemoveToken();
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
        _tokenAccess.RemoveToken();
        Dispatcher.Dispatch(async () => { await Shell.Current.GoToAsync("///login"); });
    }
}