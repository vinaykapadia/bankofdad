namespace BankOfDad.Models;

public class TransactionRequest
{
    public int From { get; set; }
    public int To { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}
