using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Collaborate.Auth.Api.Tests;

public sealed class DocumentsEndpointTests : IClassFixture<CollaborateAuthWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DocumentsEndpointTests(CollaborateAuthWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetDocument_WithValidScope_ReturnsOk()
    {
        var token = TestAuthHelper.CreateToken("documents:read");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/documents/doc-123");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDocument_WithoutToken_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/documents/doc-123");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetDocument_WithWrongScope_ReturnsForbidden()
    {
        var token = TestAuthHelper.CreateToken("comments:read");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/documents/doc-123");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetDocument_WithExpiredToken_ReturnsUnauthorized()
    {
        var token = TestAuthHelper.CreateToken(
            "documents:read",
            expires: DateTime.UtcNow.AddMinutes(-5),
            notBefore: DateTime.UtcNow.AddMinutes(-10));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/documents/doc-123");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetDocument_WithWrongAudience_ReturnsUnauthorized()
    {
        var token = TestAuthHelper.CreateToken("documents:read", audience: "wrong-audience");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/documents/doc-123");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
