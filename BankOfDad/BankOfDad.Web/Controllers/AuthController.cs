using BankOfDad.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankOfDad.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly Authentication _auth;

    public AuthController(TokenService tokenService, Authentication auth)
    {
        _tokenService = tokenService;
        _auth = auth;
    }

    [HttpPost]
    public AuthResponse Post([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return null;
        }

        var account = _auth.GetAccount(request.Username, request.Password);

        if (account == null)
        {
            return null;
        }

        var accessToken = _tokenService.CreateToken(account);
        
        return new AuthResponse
        {
            UserName = account.UserName,
            Email = account.Email,
            Token = accessToken,
        };
    }

    [HttpGet("hash/{password}")]
    public string Get(string password) => _tokenService.HashPassword(password);
}
