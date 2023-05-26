using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankOfDad.Dadabase.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BankOfDad.Web;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private const int ExpirationMinutes = 60;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration.GetSection("AuthenticationConfiguration");
    }
    
    /// <summary>
    /// Hash the password with a salt.
    /// </summary>
    /// <param name="password">Password to hash</param>
    /// <returns>Base64 string for hash + salt byte arrays</returns>
    public string HashPassword(string password)
    {
        // Generate salt
        var salt = new byte[128 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        // Hash password
        var hashedBytes = GenerateHash(password, salt);

        // Combine password and salt
        return Convert.ToBase64String(hashedBytes.Concat(salt).ToArray());
    }

    /// <summary>
    /// Check password against hashed password
    /// </summary>
    /// <param name="hash">The combined hash and salt, as a base64 string</param>
    /// <param name="password">The password to check</param>
    /// <returns>Whether the password matches the hash</returns>
    public bool VerifyHashedPassword(string hash, string password)
    {
        // Split password and salt
        var totalBytes = Convert.FromBase64String(hash);
        var savedHash = totalBytes.Take(256 / 8);
        var salt = totalBytes.Skip(256 / 8).ToArray();

        // Hash supplied password
        var checkHash = GenerateHash(password, salt);

        // Check against database password
        return savedHash.SequenceEqual(checkHash);
    }

    public string CreateToken(BankAccount user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public ClaimsIdentity GetIdentity(BankAccount user)
    {
        var claims = CreateClaims(user);
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return claimsIdentity;
    }

    private static JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
        DateTime expiration) =>
        new(
            "apiWithAuthBackend",
            "apiWithAuthBackend",
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private static List<Claim> CreateClaims(BankAccount user)
    {
        try
        {
            var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new(ClaimTypes.Name, user.Name),
                    new(ClaimTypes.Email, user.UserName),
                    new(ClaimTypes.Role, user.Balance == null ? "Administrator" : "User")
                };
            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var secret = _configuration["Secret"]
                     ?? throw new ConfigurationErrorsException("Missing authentication secret.");
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }

    private static byte[] GenerateHash(string password, byte[] salt) =>
        KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8);
}
