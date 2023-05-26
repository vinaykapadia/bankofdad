namespace BankOfDad.Models;

public class BankAccountResponse
{
    public int BankAccountId { get; set; }

    public string Name { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public decimal? Balance { get; set; }

    public IList<TransactionResponse> Transactions { get; set; }
}