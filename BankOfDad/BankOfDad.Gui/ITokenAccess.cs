namespace BankOfDad.Gui;

public interface ITokenAccess
{
    Task<string> GetToken();

    Task SetToken(string token);

    void RemoveToken();
}
