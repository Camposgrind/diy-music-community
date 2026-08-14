using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiyMusicCommunity.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DiyMusicCommunity.Infrastructure.Auth;

/// <summary>
/// Implements <see cref="IJwtTokenService"/> using <see cref="JwtSecurityTokenHandler"/>.
/// Configuration is read from the "Jwt" section in appsettings.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private const int DefaultExpiryMinutes = 60;

    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _key;
    private readonly int _expiryMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        _audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is not configured.");
        _key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured.");

        _expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var minutes)
            ? minutes
            : DefaultExpiryMinutes;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(Guid userId, string username, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_expiryMinutes);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return (token, expiresAt);
    }
}
