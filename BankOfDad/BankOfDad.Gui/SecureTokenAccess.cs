namespace BankOfDad.Gui;

public class SecureTokenAccess : ITokenAccess
{
    public async Task<string> GetToken() => await SecureStorage.GetAsync("token");

    public async Task SetToken(string token)
    {
        await SecureStorage.SetAsync("token", token);
    }

    public void RemoveToken()
    {
        SecureStorage.Remove("token");
    }
}
