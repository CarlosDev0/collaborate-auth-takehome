using Microsoft.AspNetCore.Authorization;

namespace Collaborate.Auth.Api.Authorization;

public sealed class ScopeRequirement(string scope) : IAuthorizationRequirement
{
    public string Scope { get; } = scope;
}
