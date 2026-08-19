using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Collaborate.Auth.Api.Tests;

public sealed class CollaborateAuthWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = TestAuthHelper.TestIssuer,
                ["Jwt:Audience"] = TestAuthHelper.TestAudience,
                ["Jwt:SigningKey"] = TestAuthHelper.TestSigningKey
            });
        });
    }
}
