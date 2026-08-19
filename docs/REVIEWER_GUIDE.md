# Reviewer Guide — Collaborate Take-Home Submission

**Candidate:** Carlos Sanchez  
**Exercise:** Senior Developer, Collaborate — Architecture & Implementation  
**Estimated review time:** 15–20 minutes

---

## What was submitted

| Part | Deliverable | Location |
|------|-------------|----------|
| **Part 1 (primary)** | Design document | `docs/DESIGN.md` or `docs/DESIGN.pdf` |
| **Part 2 (Option A)** | Protected resource endpoint + tests | `src/Collaborate.Auth.Api/`, `tests/` |
| **Diagram** | Architecture (Mermaid) | `diagrams/architecture.mmd` |
| **Context** | Approach, tradeoffs, AI usage | `README.md` |

**Option chosen:** **A** — A resource endpoint that serves requests only when the JWT carries the correct scope, and rejects appropriately otherwise.

**Intentionally not built:** Full IdP, login UI, permission database, Redis, or token exchange (described in the design doc as future phases).

---

## 5-minute verification

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or compatible with `net10.0` in project files)

### Run automated tests

From the repository root:

```bash
dotnet test
```

**Expected:** All tests pass. Coverage includes:

| Test | HTTP status |
|------|-------------|
| Valid token with `documents:read` scope | 200 |
| No `Authorization` header | 401 |
| Valid token with wrong scope | 403 |
| Expired token | 401 |
| Wrong audience | 401 |

### Run the API

```bash
dotnet run --project src/Collaborate.Auth.Api
```

API listens on **`http://localhost:5075`**.

- OpenAPI (Development): `http://localhost:5075/openapi/v1.json`
- Protected endpoint: `GET /api/documents/{id}`

No Auth0, database, or frontend setup required.

---

## What to read first (design)

Open **`docs/DESIGN.md`** — it addresses all five Part 1 sections:

1. **High-level architecture** — Login (PKCE), permission checking (cache + `perm_version` revocation), on-behalf-of (RFC 8693 token exchange, confused-deputy controls)
2. **Implementation plan** — Four phased rollout
3. **Testing strategy** — Unit, integration, contract, load, security
4. **Evaluation & observability** — Metrics, logging, tracing, alerts
5. **Failure modes & tradeoffs** — Cache consistency, TTL, framework vs custom crypto

---

## What to inspect in code (Part 2)

| File | Purpose |
|------|---------|
| `src/Collaborate.Auth.Api/Program.cs` | JWT Bearer setup, authorization policies |
| `src/Collaborate.Auth.Api/Controllers/DocumentsController.cs` | Protected `GET /api/documents/{id}` |
| `src/Collaborate.Auth.Api/Authorization/ScopeAuthorizationHandler.cs` | Maps OAuth2 `scope` claim to policies |
| `tests/Collaborate.Auth.Api.Tests/DocumentsEndpointTests.cs` | End-to-end 200 / 401 / 403 scenarios |

**Design choice:** ASP.NET Core handles signature verification, issuer/audience/lifetime, and clock skew. Custom code only enforces domain scope rules — aligned with the exercise guidance to avoid hand-rolled cryptography.

---

## Optional manual API test

Generate a JWT (e.g. [jwt.io](https://jwt.io)) with **HS256** and these values from `appsettings.json`:

| Field | Value |
|-------|-------|
| Algorithm | HS256 |
| `scope` | `documents:read` |
| `iss` | `https://auth.collaborate.test` |
| `aud` | `https://collaborate-api` |
| Secret | Value of `Jwt:SigningKey` in `appsettings.json` |
| `exp` | Future Unix timestamp |

```http
GET http://localhost:5075/api/documents/doc-123
Authorization: Bearer <token>
```

Use **HTTP** (not HTTPS) in Development to avoid redirect issues with Bearer tokens.

---

## AI usage (for follow-up interview)

Documented in **`README.md` → AI Usage Notes**. Summary:

- AI helped with scaffolding and design doc drafting.
- Candidate applied judgment on Option A scope, framework choices, and security tradeoffs.
- AI output was reviewed for crypto/OAuth2 correctness before use.

---

## Questions worth asking in the live review

- Why Option A over B or C for a 2–3 hour budget?
- How would `perm_version` revocation work without a DB call on every request?
- How would production move from HS256 (dev stub) to RS256/JWKS from the IdP?
- What confused-deputy controls apply to the on-behalf-of token exchange design?

---

## Troubleshooting

| Issue | Likely cause |
|-------|----------------|
| Tests fail to build | .NET SDK version mismatch — check `TargetFramework` in `.csproj` |
| 401 on manual test | Wrong issuer, audience, secret, or expired `exp` |
| Connection refused | API not running on port 5075 |

For full developer setup details, see the root **`README.md`**.
