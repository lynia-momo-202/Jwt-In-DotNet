using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MonAPI.Data;
using MonAPI.Keys;

namespace MonAPI.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;
    private readonly MaBaseDbContext _context;

    public JwtService(IConfiguration configuration, MaBaseDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public string GenerateToken(string userName, string audience, List<Claim> userClaims)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64 )
        }.Union(userClaims);

        var credentials = new SigningCredentials(new RsaSecurityKey(RsaKeyProvider.GetPrivateKey()), SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public async Task SaveRefreshToken(string userName, string token)
    {
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserName = userName,
            Expires = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsValidRefreshToken(string token)
    {
        var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);

        return refreshToken != null && refreshToken.Expires > DateTime.UtcNow;
    }

    public async Task RevokeRefreshToken(string token)
    {
        var storedToken = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);

        if (storedToken != null)
        {
            storedToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }

    public bool IsValidAudience(string audience)
   => !string.IsNullOrEmpty(audience) && _context.AuthorizedApplications.Any(a => a.Audience.ToUpper().Equals(audience));
   
   public string[] GetValidAudiences()
   => _context.AuthorizedApplications.Select(a => a.Audience).ToArray();
}
