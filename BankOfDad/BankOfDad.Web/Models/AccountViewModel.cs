using BankOfDad.Models;

namespace BankOfDad.Web.Models;

public class AccountViewModel
{
    public BankAccountResponse BankAccount { get; set; }
    public Dictionary<int, string> ToAccounts { get; set; }
    public TransactionRequest Request { get; set; }
}
