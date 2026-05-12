using Application.DTOs;
using Infrastructure.Data.Identity;
using Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services;

public class JwtTokenService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _options = options.Value;

    public AccessTokenResult CreateAccessToken(AppUser user, IList<string> roles)
    {
        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtClaimTypes.UserId, user.Id),
            new(JwtClaimTypes.Email, user.Email ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

        var credentials = new SigningCredentials(_options.GetSigningKey(), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: credentials
            );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult
            (
                AccessToken: accessToken,
                TokenType: "Bearer",
                Expires: (int)(expiresAtUtc - now).TotalSeconds,
                ExpiresAtUtc: expiresAtUtc
            );
    }
}
