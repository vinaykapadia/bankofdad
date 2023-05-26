using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618
namespace BankOfDad.Dadabase.Models;

[Table(nameof(Transaction))]
public class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(nameof(TransactionId))]
    public int TransactionId { get; set; }

    [Column(nameof(Description))]
    [MaxLength(100)]
    public string Description { get; set; }

    [Column(nameof(TimeStamp))]
    public DateTime TimeStamp { get; set; }

    [Column(nameof(Amount))]
    public int Amount { get; set; }

    [Column(nameof(Balance))]
    public int? Balance { get; set; }

    [Column(nameof(DestinationAccount))]
    [MaxLength(30)]
    public string DestinationAccount { get; set; }

    [ForeignKey(nameof(BankAccountId))]
    public virtual BankAccount BankAccount { get; set; }
    public int BankAccountId { get; set; }
}