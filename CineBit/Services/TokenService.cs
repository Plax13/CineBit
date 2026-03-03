using CineBit.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    private const int ExpirationMinutes = 30;
    private readonly ILogger<TokenService> _logger;
    private readonly IConfiguration _configuration;

    public TokenService(ILogger<TokenService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public string CreateToken(Utente user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        _logger.LogInformation("JWT Token created");
        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(
        List<Claim> claims,
        SigningCredentials credentials,
        DateTime expiration)
    {
        var jwtSettings = _configuration.GetSection("JwtTokenSettings");

        return new JwtSecurityToken(
            issuer: jwtSettings["ValidIssuer"],
            audience: jwtSettings["ValidAudience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );
    }

    private List<Claim> CreateClaims(Utente user)
    {
        var jwtSub = _configuration.GetSection("JwtTokenSettings")["JwtRegisteredClaimNamesSub"]
                     ?? throw new InvalidOperationException("JwtRegisteredClaimNamesSub mancante");

        return new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, jwtSub),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Ruolo.ToString())
        };
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var secretKey = _configuration.GetSection("JwtTokenSettings")["SymmetricSecurityKey"]
                        ?? throw new InvalidOperationException("SymmetricSecurityKey mancante");

        return new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256
        );
    }
}