# Collaborate Authorization Layer — Take-Home Exercise

ASP.NET Core implementation of **Option A**: a protected resource endpoint that serves requests only when the caller's JWT carries the correct scope, and rejects otherwise.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or compatible SDK matching `TargetFramework` in the project files)

## Run the API

```bash
cd C:\CARLOS\Programming\.NET\Original_Test
dotnet run --project src/Collaborate.Auth.Api
```

The API exposes:

- `GET /api/documents/{id}` — requires Bearer token with scope `documents:read`
- OpenAPI spec at `/openapi/v1.json` (Development environment)

## Run Tests

```bash
dotnet test
```

Integration tests cover:

| Scenario | Expected |
|----------|----------|
| Valid token with `documents:read` | 200 OK |
| No Authorization header | 401 Unauthorized |
| Valid token with wrong scope | 403 Forbidden |
| Expired token | 401 Unauthorized |
| Wrong audience | 401 Unauthorized |

## Manual Testing

Generate a test JWT using the same parameters as `appsettings.json` (issuer, audience, signing key) with a `scope` claim of `documents:read`, then call:

```http
GET http://localhost:5075/api/documents/doc-123
Authorization: Bearer <token>
```

**Insomnia tip:** Use the **HTTP** URL (`http://localhost:5075`), not HTTPS. In Development, HTTPS redirection is disabled so Bearer tokens are not dropped by redirect behavior. Your jwt.io token parameters are correct if they match `appsettings.Development.json`.

## Option A — Approach & Justification

**What was built:** A Document Service stub demonstrating how downstream Collaborate APIs enforce authorization using token scopes/claims — matching the assignment requirement that resource APIs validate tokens locally without querying the permissions database.

**Framework choices:**

| Component | Choice | Why |
|-----------|--------|-----|
| Token validation | `Microsoft.AspNetCore.Authentication.JwtBearer` | Handles signature verification, issuer/audience/lifetime checks, clock skew, and JWKS integration in production |
| Authorization | Policy-based `[Authorize]` + custom `ScopeAuthorizationHandler` | Domain-specific scope checks; no custom cryptography |
| Tests | `WebApplicationFactory` + `System.IdentityModel.Tokens.Jwt` | End-to-end validation of 401/403 behavior |

Custom code is limited to mapping OAuth2 `scope` claims to ASP.NET Core authorization policies. Hand-rolling JWT parsing or key management would add risk without benefit at this layer.

**Tradeoff:** Dev environment uses a symmetric signing key (HS256) for simplicity. Production would use asymmetric keys (RS256/ES256) from the IdP's JWKS endpoint.

## Project Structure

```
src/Collaborate.Auth.Api/     — Web API (Document endpoint + JWT auth)
tests/Collaborate.Auth.Api.Tests/ — Integration tests
docs/DESIGN.md                — Part 1 architecture & design document
diagrams/architecture.mmd     — Mermaid architecture diagram source
```

## Design Document

See [docs/DESIGN.md](docs/DESIGN.md) for the full Part 1 design covering login, permission checking, on-behalf-of flows, testing strategy, observability, and failure modes.

To export to PDF:

- **Pre-generated:** [`docs/DESIGN.pdf`](docs/DESIGN.pdf) (generated from `docs/DESIGN.html`)
- **Regenerate:** Open `docs/DESIGN.html` in a browser and Print to PDF, or use Edge headless:

```powershell
& "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe" `
  --headless --disable-gpu --no-pdf-header-footer `
  --print-to-pdf="docs/DESIGN.pdf" `
  "file:///C:/CARLOS/Programming/.NET/Original_Test/docs/DESIGN.html"
```

## Submission materials

- **[Reviewer guide](docs/REVIEWER_GUIDE.md)** — quick start for hiring panel (tests, what to read, what to ask)


## Assumptions

- Caseware central IdP and firm-federated IdPs are external; not implemented here.
- Permission DB, Redis cache, and token exchange are described in the design doc but not built in this slice.
- JWT signing key in `appsettings.json` is for local development only — use environment variables or a secrets manager in production.

## AI Usage Notes (for follow-up review)

- AI assisted with project scaffolding, boilerplate structure, and design doc drafting.
- Human judgment applied to: choosing Option A for time budget, leaning on ASP.NET Core built-in JWT middleware rather than custom crypto, scope-based policy design aligned with OAuth2 conventions, and fail-closed revocation strategy in the design doc.
- AI should not be trusted for: cryptographic parameter choices, OAuth2 spec edge cases, or production security configuration without manual review.
