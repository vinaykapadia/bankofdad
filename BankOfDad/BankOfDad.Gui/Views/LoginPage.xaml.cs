using BankOfDad.Client;

namespace BankOfDad.Gui.Views;

public partial class LoginPage : ContentPage
{
    private readonly BankOfDadClient _client;
    private readonly ITokenAccess _tokenAccess;

    public LoginPage(BankOfDadClient client, ITokenAccess tokenAccess)
	{
        _client = client;
        _tokenAccess = tokenAccess;

		InitializeComponent();
	}

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        var token = await GetToken(Username.Text, Password.Text);

        if (token != null)
        {
            await _tokenAccess.SetToken(token);
            _client.SetToken(token);
            await Shell.Current.GoToAsync("///home");
        }
        else
        {
            await DisplayAlert("Login failed", "Username or password invalid", "Try again");
        }
    }
    
    private async Task<string> GetToken(string username, string password)
    {
        var response = await _client.PostAuth(username, password);
        return response?.Token;
    }
}