# ADR 001 — Secrets Management via Azure Key Vault

## Status
Accepted

## Context
The application requires several sensitive configuration values:
- JWT signing key
- Database connection string (including credentials)
- Any future third-party API keys

These values must never be committed to the repository (even in a private repo), as doing so
creates an irreversible leak risk and violates least-privilege principles.

A consistent, auditable secrets management strategy is needed that works across local development,
CI/CD pipelines, and production.

## Decision
**Use Azure Key Vault as the single source of truth for all secrets production.**

- **Local development:** .NET User Secrets (`dotnet user-secrets`) for backend; no secrets in frontend.
- **Production:** Azure Key Vault, accessed at runtime via `AddAzureKeyVault()` in `Program.cs`.
- **Authentication to Key Vault:** Managed Identity in Azure environments; `DefaultAzureCredential` locally (requires `az login` + Key Vault Secrets User role grant).
- **`appsettings.json` and all committed config files** contain only placeholder strings (e.g. `"SET_VIA_KEYVAULT_OR_USER_SECRETS"`), never real values.
- **Secret naming convention in Key Vault:** `--` as hierarchy separator (e.g. `JwtSettings--Secret` maps to `JwtSettings:Secret` in .NET configuration).
- **Frontend:** `environment.ts` files contain only non-sensitive config. No secrets ever on the client.

## Consequences
- **Positive:** secrets are never in source control; rotation is done in Key Vault without a redeploy; access is auditable via Key Vault access logs.
- **Positive:** `DefaultAzureCredential` supports both local dev and managed identity with no code change.
- **Trade-off:** developers need `az login` and the Key Vault Secrets User role to run the app locally against real secrets. For pure local dev without Azure, User Secrets suffice.
- **Trade-off:** Azure Key Vault is an MVP dependency for deployed environments, but adds no complexity to the local dev loop.
- **Out of scope for MVP:** secret rotation automation, Key Vault references in App Service config (can be added later), HashiCorp Vault as an alternative.