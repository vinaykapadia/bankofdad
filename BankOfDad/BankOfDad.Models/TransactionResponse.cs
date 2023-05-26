namespace BankOfDad.Models;

public class TransactionResponse
{
    public int TransactionId { get; set; }

    public string Description { get; set; }

    public DateTime TimeStamp { get; set; }

    public decimal Amount { get; set; }

    public decimal? Balance { get; set; }

    public string DestinationAccount { get; set; }
}