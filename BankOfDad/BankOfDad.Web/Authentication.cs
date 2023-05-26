using BankOfDad.Dadabase;
using BankOfDad.Dadabase.Models;

namespace BankOfDad.Web;

public class Authentication
{
    private readonly TokenService _tokenService;
    private readonly BankOfDadContext _db;

    public Authentication(BankOfDadContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public BankAccount GetAccount(string username, string password)
    {
        var account = _db.BankAccounts.FirstOrDefault(a => a.UserName == username);

        if (account == null)
        {
            return null;
        }

        var isPasswordValid = _tokenService.VerifyHashedPassword(account.PasswordHash, password);
        return isPasswordValid ? account : null;
    }
}
