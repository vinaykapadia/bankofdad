#pragma warning disable CS8618
using BankOfDad.Dadabase.Models;
using Microsoft.EntityFrameworkCore;

namespace BankOfDad.Dadabase;

public class BankOfDadContext : DbContext
{
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public BankOfDadContext(DbContextOptions options)
        : base(options)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>()
            .HasMany(bankAccount => bankAccount.Transactions)
            .WithOne(transaction => transaction.BankAccount);
    }
}