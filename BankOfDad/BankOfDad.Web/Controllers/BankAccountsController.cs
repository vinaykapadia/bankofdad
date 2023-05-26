using BankOfDad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankOfDad.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BankAccountsController : ControllerBase
    {
        private readonly DatabaseAccess _db;

        public BankAccountsController(DatabaseAccess db)
        {
            _db = db;
        }
        
        // GET: api/<BankAccountsController>
        [HttpGet]
        public IEnumerable<BankAccountResponse> GetAccounts() => _db.GetAccounts();

        [HttpGet("me")]
        public BankAccountResponse GetAccount() => _db.GetAccount();

        // GET api/<BankAccountsController>/5
        [HttpGet("{id}")]
        public BankAccountResponse GetAccount(int id) => _db.GetAccount(id);

        [HttpPost("transaction")]
        public TransactionResponse AddTransaction([FromBody]TransactionRequest request)
        {
            var amount = (ushort)Math.Floor(request.Amount * 100);
            return _db.AddTransaction(request.From, request.To, amount, request.Description);
        }
    }
}
