# Skill: Security Review

## Purpose
Checklist for reviewing authentication, authorization, data-access security, and secrets hygiene
in the DIY Music Community project before merging any PR that touches auth, permissions, data
exposure, or configuration.

## Boundary
- Never weaken auth to make tests pass.
- Never expose passwords, tokens, or secrets in responses, logs, or committed files.

## Checklist

### Secrets & configuration
- [ ] No passwords, JWT keys, connection strings, or API keys are hardcoded in any source file.
- [ ] `appsettings.json` / `appsettings.*.json` contain only placeholder values (e.g. `SET_VIA_KEYVAULT_OR_USER_SECRETS`).
- [ ] Local dev secrets use .NET User Secrets (`dotnet user-secrets`) — not `appsettings.Development.json`.
- [ ] Staging/production secrets are in Azure Key Vault; app uses `AddAzureKeyVault()` + Managed Identity.
- [ ] `.gitignore` covers `appsettings.Local.json`, `.env`, `.env.*`, `secrets.json`, `local.settings.json`.
- [ ] No secret value appears in logs, error responses, or API payloads.
- [ ] `environment.ts` / `environment.prod.ts` contain no secrets or API keys.

### Authentication
- [ ] JWT is validated (signature, expiry, issuer, audience) on every protected endpoint.
- [ ] Passwords are hashed with bcrypt / PBKDF2 — never stored plain.
- [ ] Tokens are not logged or returned in error responses.
- [ ] Login endpoint returns the same error for wrong email and wrong password (no user enumeration).

### Authorization — Role-based
- [ ] Moderation endpoints require `[Authorize(Roles = "Moderator,Admin")]`.
- [ ] User-only endpoints require `[Authorize]` (any authenticated user).
- [ ] Anonymous endpoints are intentionally left without `[Authorize]`.
- [ ] 401 (unauthenticated) vs 403 (authenticated but forbidden) are returned correctly.

### Authorization — Resource-based (band claims)
- [ ] Band edit access uses `IBandAccessService.CanEditBand(userId, bandId)` — checks for an `Approved` BandClaim row.
- [ ] This check is NOT replaced by a global role.
- [ ] The JWT never contains claim-ownership information.

### Data exposure
- [ ] API responses return DTOs, never EF entity objects.
- [ ] No `PasswordHash` or sensitive fields leak into any DTO.
- [ ] Pagination is enforced on list endpoints (no unbounded queries).
- [ ] Blocked bands are filtered out of public list responses.

### Input validation
- [ ] All commands / DTOs have FluentValidation validators.
- [ ] File upload paths (future) are sanitized.

### Tests
- [ ] A test exists that verifies a 403 is returned when an unauthorized role calls a protected endpoint.
- [ ] A test exists that verifies a 401 is returned when no token is provided to a protected endpoint.

## What NOT to do during security review
- Do not add a global "band owner" role to JWT — ownership is per-band.
- Do not disable HTTPS redirection or CORS wildcard (`*`) for convenience.
- Do not store the JWT in a cookie without `HttpOnly` + `SameSite` (out of scope for MVP — use localStorage with the known trade-offs).
- Do not "temporarily" hardcode a secret to unblock development.
- Do not commit `.env` files or `appsettings.Local.json` even if the repo is private.