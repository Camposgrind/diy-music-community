# Application Insights — Live setup

## What the API sends

When configured, the API automatically sends request rate, failed requests,
response duration, dependency calls, unhandled exceptions, and `Warning`/
`Error` application logs. It additionally tracks successful registrations,
logins, and catalog mutations as the business events and metrics defined in
[`013-application-insights-observability.md`](../specs/013-application-insights-observability.md).

No custom telemetry contains personal data, secrets, entity IDs, request bodies,
file names, blob paths, or signed URLs.

## Live configuration

1. In the existing Application Insights resource, copy its **connection string**.
   Treat it as a secret; do not add it to `appsettings*.json`, GitHub secrets,
   source code, tickets, or logs.
2. In the production Key Vault, create a secret named
   `ApplicationInsights--ConnectionString` and set the copied connection string
   as its value. The double hyphen maps to `ApplicationInsights:ConnectionString`
   in .NET configuration.
3. In the API App Service, enable its system-assigned managed identity.
4. Grant that managed identity the **Key Vault Secrets User** role on the Key
   Vault. If the vault uses access policies instead of Azure RBAC, grant the
   identity the equivalent secret `Get` permission.
5. Confirm the App Service has a non-secret application setting named
   `AzureKeyVaultEndpoint` whose value is the Key Vault URI. Do not store the
   Application Insights connection string in App Service settings.
6. Deploy this branch. `Program.cs` loads Key Vault before registering Application
   Insights, so the secret is resolved at startup through the managed identity.
7. Make a request to the deployed API, then wait a few minutes and verify data in
   Application Insights **Logs**.

For local development, use .NET User Secrets rather than a local settings file:

```powershell
cd backend/src/DiyMusicCommunity.Api
dotnet user-secrets set "ApplicationInsights:ConnectionString" "<connection-string>"
```

Remove it when it is no longer needed:

```powershell
dotnet user-secrets remove "ApplicationInsights:ConnectionString"
```

## Useful KQL queries

Failed requests in the last 24 hours:

```kusto
requests
| where timestamp > ago(24h) and success == false
| summarize failures = count() by name, resultCode
| order by failures desc
```

Unhandled exceptions:

```kusto
exceptions
| where timestamp > ago(24h)
| summarize occurrences = count() by type, outerMessage
| order by occurrences desc
```

Business events:

```kusto
customEvents
| where timestamp > ago(24h)
| where name in ("UserRegistered", "UserLoginSucceeded", "BandCreated", "ReleaseCreated")
| summarize count() by name, bin(timestamp, 1h)
| order by timestamp asc
```

Business metrics:

```kusto
customMetrics
| where timestamp > ago(24h)
| where name in ("UsersRegistered", "SuccessfulLogins", "BandsCreated", "ReleasesCreated")
| summarize total = sum(value) by name, bin(timestamp, 1h)
| order by timestamp asc
```

## Initial alerts and cost controls

- Alert when failed requests exceed 5 in 5 minutes; refine the threshold after
  observing normal traffic.
- Alert when any unhandled exception occurs in 5 minutes.
- Alert when the API's 95th-percentile response time exceeds 2 seconds for
  10 minutes.
- Set a daily ingestion cap and review adaptive sampling before enabling verbose
  `Information` logging in Live.
- Review the Application Insights retention period and the alert action group
  recipients as part of the production handoff.
