using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618
namespace BankOfDad.Dadabase.Models;

[Table(nameof(BankAccount))]
public class BankAccount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(nameof(BankAccountId))]
    public int BankAccountId { get; set; }

    [Column(nameof(Name))]
    [MaxLength(30)]
    public string Name { get; set; }
    
    [Column(nameof(UserName))]
    [MaxLength(20)]
    public string UserName { get; set; }
    
    [Column(nameof(Email))]
    [MaxLength(30)]
    public string Email { get; set; }
    
    [Column(nameof(PasswordHash))]
    [MaxLength(100)]
    public string PasswordHash { get; set; }
    
    [Column(nameof(Balance))]
    public int? Balance { get; set; }
    
    public virtual IList<Transaction> Transactions { get; set; }
}