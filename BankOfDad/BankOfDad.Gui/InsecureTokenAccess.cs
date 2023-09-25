namespace BankOfDad.Gui;

public class InsecureTokenAccess : ITokenAccess
{
    private string _insecureToken;

    public Task<string> GetToken() => Task.FromResult(_insecureToken);

    public Task SetToken(string token)
    {
        _insecureToken = token;
        return Task.CompletedTask;
    }

    public void RemoveToken()
    {
        _insecureToken = null;
    }
}
