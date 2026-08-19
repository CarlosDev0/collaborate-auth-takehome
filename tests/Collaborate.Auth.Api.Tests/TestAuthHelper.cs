using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Collaborate.Auth.Api.Tests;

public static class TestAuthHelper
{
    public const string TestSigningKey = "CollaborateDevSigningKeyMustBe32BytesLong!";
    public const string TestIssuer = "https://auth.collaborate.test";
    public const string TestAudience = "collaborate-api";

    public static string CreateToken(
        string scope,
        string? audience = null,
        DateTime? expires = null,
        DateTime? notBefore = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "test-user-id"),
            new("scope", scope)
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: audience ?? TestAudience,
            claims: claims,
            notBefore: notBefore ?? DateTime.UtcNow.AddMinutes(-1),
            expires: expires ?? DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
