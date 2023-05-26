using AutoMapper;
using BankOfDad.Dadabase.Models;

namespace BankOfDad.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<int, decimal>().ConvertUsing(i => i / 100.0M);
        CreateMap<BankAccount, BankAccountResponse>();
        CreateMap<Transaction, TransactionResponse>();
    }
}