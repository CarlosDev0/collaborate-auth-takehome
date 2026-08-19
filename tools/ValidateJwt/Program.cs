using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

const string signingKey = "CollaborateDevSigningKeyMustBe32BytesLong!";
const string issuer = "https://auth.collaborate.test";
const string audience = "collaborate-api";
const string userToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0LXVzZXItaWQiLCJzY29wZSI6ImRvY3VtZW50czpyZWFkIiwiaXNzIjoiaHR0cHM6Ly9hdXRoLmNvbGxhYm9yYXRlLnRlc3QiLCJhdWQiOiJjb2xsYWJvcmF0ZS1hcGkiLCJleHAiOjE4OTM0NTYwMDAsIm5iZiI6MTYwOTQ1OTIwMH0.2OUF_7AqKvBKeYXnHGW_ViaGWetLSzonmfR1fWAIC5A";

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
var parameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = issuer,
    ValidAudience = audience,
    IssuerSigningKey = key,
    ClockSkew = TimeSpan.FromMinutes(1)
};

Validate("jwt.io token", userToken, parameters);

var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
var claims = new List<Claim>
{
    new(JwtRegisteredClaimNames.Sub, "test-user-id"),
    new("scope", "documents:read")
};
var dotnetToken = new JwtSecurityToken(
    issuer: issuer,
    audience: audience,
    claims: claims,
    notBefore: DateTime.UtcNow.AddMinutes(-1),
    expires: DateTime.UtcNow.AddMinutes(5),
    signingCredentials: credentials);
var dotnetTokenString = new JwtSecurityTokenHandler().WriteToken(dotnetToken);
Validate("dotnet token", dotnetTokenString, parameters);

Console.WriteLine($"dotnet token: {dotnetTokenString}");

static void Validate(string label, string token, TokenValidationParameters parameters)
{
    var handler = new JwtSecurityTokenHandler();
    try
    {
        handler.ValidateToken(token, parameters, out _);
        Console.WriteLine($"{label}: VALID");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{label}: INVALID - {ex.GetType().Name}: {ex.Message}");
    }
}
