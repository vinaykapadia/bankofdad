using BankOfDad.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using BankOfDad.Models;

namespace BankOfDad.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseAccess _db;
        private readonly Authentication _authentication;
        private readonly TokenService _tokenService;

        public HomeController(DatabaseAccess db, Authentication authentication, TokenService tokenService)
        {
            _db = db;
            _authentication = authentication;
            _tokenService = tokenService;

            CultureInfo culture = new CultureInfo("en-us");
            culture.NumberFormat.CurrencyNegativePattern = 1;

            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(AuthRequest request)
        {
            var account = _authentication.GetAccount(request.Username, request.Password);

            if (account == null)
            {
                return RedirectToAction("Index");
            }

            var claimsIdentity = _tokenService.GetIdentity(account);
            
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                // Refreshing the authentication session should be allowed.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Account");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Account()
        {
            var account = _db.GetAccount();
            var viewModel = new AccountViewModel
            {
                BankAccount = account,
                ToAccounts = _db.GetAccounts().Where(x => x.BankAccountId != account.BankAccountId).ToDictionary(x => x.BankAccountId, y => y.Name),
                Request = new TransactionRequest
                {
                    From = account.BankAccountId
                }
            };
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Transact(AccountViewModel model)
        {
            var decimalAmount = Math.Floor(model.Request.Amount * 100);
            if (ushort.TryParse(decimalAmount.ToString(CultureInfo.InvariantCulture), out var amount)
                && amount > 0)
            {
                _db.AddTransaction(model.Request.From, model.Request.To, amount, model.Request.Description);
            }

            return RedirectToAction("Account");
        }
    }
}