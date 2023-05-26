using BankOfDad.Client;

namespace BankOfDad.Gui.Views;

public partial class TransactionPage : ContentPage
{
    private readonly BankOfDadClient _client;
    private int accountId;

    public TransactionPage(BankOfDadClient client)
    {
        _client = client;

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
        var token = SecureStorage.GetAsync("token").Result;
        _client.SetToken(token);

        var account = await _client.GetAccount();

        if (account == null)
        {
            SecureStorage.Remove("token");
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
        SecureStorage.Remove("token");
        Dispatcher.Dispatch(async () => { await Shell.Current.GoToAsync("///login"); });
    }
}