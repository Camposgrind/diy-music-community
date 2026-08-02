---
applyTo: "**/*.json,**/*.env*,**/*.cs,**/*.ts,**/*.yml,**/*.yaml"
---

# Secrets & Sensitive Data Instructions — DIY Music Community

## Golden rule
**Never hardcode passwords, API keys, connection strings, JWT secrets, or any sensitive value
in source code, appsettings files, or any file committed to the repository.**

## What counts as a secret
- Database connection strings (including username + password).
- JWT signing keys (`JwtSettings:Secret`, `JwtSettings:Key`, etc.).
- Any third-party API key or client secret.
- SMTP / email credentials.
- Azure Storage account keys or SAS tokens.
- Any value that would grant access to a system if leaked.

## How to reference secrets correctly

### Backend (ASP.NET Core)
- **Development:** use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
  (`dotnet user-secrets set "JwtSettings:Secret" "..."`) — stored outside the repo in `%APPDATA%\Microsoft\UserSecrets\`.
- **Staging / Production:** secrets are stored in **Azure Key Vault** and injected at runtime via
  `AddAzureKeyVault(...)` in `Program.cs`. The app never reads the raw value from a file.
- `appsettings.json` and `appsettings.*.json` must contain only **placeholder** values:

```json
// ✅ correct — placeholder only
{
  "JwtSettings": {
    "Secret": "SET_VIA_KEYVAULT_OR_USER_SECRETS",
    "Issuer": "diy-music-community",
    "ExpiryMinutes": 60
  },
  "ConnectionStrings": {
    "Default": "SET_VIA_KEYVAULT_OR_USER_SECRETS"
  }
}

// ❌ wrong — real value in file
{
  "JwtSettings": {
    "Secret": "my-super-secret-key-1234"
  }
}
```

### Frontend (Angular)
- `environment.ts` / `environment.prod.ts` must **never** contain API keys or secrets.
- Only non-sensitive config belongs there (e.g. `apiBaseUrl: '/api'`).
- If a frontend feature ever needs a client-side key (e.g. analytics), inject it at build time
  via a CI environment variable — never commit the value.

## Azure Key Vault — conventions
- Key Vault name: configured in `appsettings.json` as `"KeyVaultName": "diy-music-community-kv"` (the name is not a secret).
- Secret naming in Key Vault uses `--` as the hierarchy separator (maps to `:` in .NET config):
  `JwtSettings--Secret`, `ConnectionStrings--Default`.
- The app authenticates to Key Vault via **Managed Identity** in Azure; locally via `DefaultAzureCredential`
  (developer must be logged in with `az login` and granted Key Vault Secrets User role).
- Never pass Key Vault credentials (client secret / certificate) through `appsettings.json`.

## .gitignore requirements
Ensure these entries exist in `.gitignore`:
```
# User secrets & local overrides
appsettings.Local.json
appsettings.Development.Local.json
.env
.env.*
!.env.example
secrets.json

# Azure credentials
local.settings.json
```

## Must NOT do
- Commit any real secret, password, key, or token to the repository — even in a private repo.
- Use `appsettings.Development.json` to store real credentials (use User Secrets instead).
- Log secrets, connection strings, or JWT payloads anywhere.
- Return secrets in API responses or error messages.
- Store secrets in Angular `environment.ts` files.
- Disable Key Vault integration "temporarily" and hardcode a value instead.