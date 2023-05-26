using AutoMapper;
using BankOfDad.Dadabase;
using BankOfDad.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BankOfDad.Dadabase.Models;
using BankOfDad.Models;

namespace BankOfDad.Web;

public class DatabaseAccess
{
    private readonly IMapper _mapper;
    private readonly BankOfDadContext _db;
    private readonly IHttpContextAccessor _context;

    public DatabaseAccess(IMapper mapper, BankOfDadContext db, IHttpContextAccessor context)
    {
        _mapper = mapper;
        _db = db;
        _context = context;
    }

    public IEnumerable<BankAccountResponse> GetAccounts()
    {
        return _mapper.Map<IList<BankAccountResponse>>(_db.BankAccounts);
    }

    public BankAccountResponse GetAccount() => GetAccount(UserName);

    public BankAccountResponse GetAccount(int id)
    {
        var account = _db.BankAccounts.Include(a => a.Transactions
                .OrderByDescending(t => t.TimeStamp))
            .FirstOrDefault(a => a.BankAccountId == id);

        if (account == null || (Role != "Administrator" && account.UserName != UserName))
        {
            return null;
        }

        return _mapper.Map<BankAccountResponse>(account);
    }
    
    public BankAccountResponse GetAccount(string username)
    {
        var account = _db.BankAccounts.Include(a => a.Transactions
                .OrderByDescending(t => t.TimeStamp))
            .FirstOrDefault(a => a.UserName == username);

        if (account == null || (Role != "Administrator" && account.UserName != UserName))
        {
            return null;
        }

        return _mapper.Map<BankAccountResponse>(account);
    }

    public TransactionResponse AddTransaction(int id, int toId, ushort amount, string description)
    {
        var a1 = _db.BankAccounts.Include(a => a.Transactions)
            .FirstOrDefault(a => a.BankAccountId == id);

        if (a1 == null || (Role != "Administrator" && a1.UserName != UserName) || a1.Balance < amount)
        {
            return null;
        }

        var a2 = _db.BankAccounts.Include(a => a.Transactions)
            .First(a => a.BankAccountId == toId);

        a1.Balance -= amount;
        a2.Balance += amount;
        var time = DateTime.Now;

        var t1 = new Transaction
        {
            TimeStamp = time,
            Amount = -amount,
            DestinationAccount = a2.Name,
            Balance = a1.Balance,
            Description = description ?? "-"
        };

        var t2 = new Transaction
        {
            TimeStamp = time,
            Amount = amount,
            DestinationAccount = a1.Name,
            Balance = a2.Balance,
            Description = description ?? "-"
        };

        a1.Transactions.Add(t1);
        a2.Transactions.Add(t2);

        _db.SaveChanges();

        return _mapper.Map<TransactionResponse>(t1);
    }

    private string UserName => GetClaim(ClaimTypes.Email);

    private string Role => GetClaim(ClaimTypes.Role);

    private string GetClaim(string claimType) =>
        _context.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? "";
}
