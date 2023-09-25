using BankOfDad.Client;

namespace BankOfDad.Gui.Views;

public partial class TransactionPage : ContentPage
{
    private readonly BankOfDadClient _client;
    private readonly ITokenAccess _tokenAccess;
    private int accountId;

    public TransactionPage(BankOfDadClient client, ITokenAccess tokenAccess)
    {
        _client = client;
        _tokenAccess = tokenAccess;

        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        var thread = new Thread(async () => { await UpdateAccount(); });
        thread.Start();

        base.OnNavigatedTo(args);
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

        accountId = account.BankAccountId;
        var allAccounts = await _client.GetAccounts();
        var toAccounts = allAccounts.Where(x => x.BankAccountId != account.BankAccountId)
            .ToDictionary(x => x.BankAccountId, y => y.Name);

        Dispatcher.Dispatch(() =>
        {
            LblName.Text = account.Name;
            LblBalance.Text = account.Balance.HasValue ? $"{account.Balance:C}" : "$∞";
            
            LstToAccounts.ItemsSource = toAccounts.ToList();
            LstToAccounts.ItemDisplayBinding = new Binding("Value");
        });
    }

    private async void BtnSend_Clicked(object sender, EventArgs args)
    {
        var toAccount = (KeyValuePair<int, string>)LstToAccounts.SelectedItem;
        var amount = decimal.Parse(TxtAmount.Text);
        var desc = TxtDescription.Text;
        TxtAmount.Text = "";
        TxtDescription.Text = "";

        await _client.AddTransaction(accountId, toAccount.Key, amount, desc);
        Dispatcher.Dispatch(async () => { await Shell.Current.GoToAsync("///home"); });
    }

    private void BtnLogout_Clicked(object sender, EventArgs args)
    {
        _tokenAccess.RemoveToken();
        Dispatcher.Dispatch(async () => { await Shell.Current.GoToAsync("///login"); });
    }
}