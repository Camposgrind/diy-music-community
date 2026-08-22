# ADR 005 — Application Insights via Key Vault

## Status

Accepted

## Context

The API needs production observability, but the Application Insights connection string is a credential that must not be committed to source control or exposed in an application settings file.

## Decision

The API uses the Azure Application Insights ASP.NET Core SDK in the API composition layer. It reads `ApplicationInsights:ConnectionString` from the existing configuration chain.

- Local development uses .NET User Secrets.
- Live uses Azure Key Vault and the App Service managed identity. The Key Vault secret is named `ApplicationInsights--ConnectionString`.
- Committed settings contain a placeholder only.
- The API sends automatic request, dependency, exception, and logging telemetry. Explicit business events and metrics are emitted only after successful user, authentication, and catalog mutations.
- Custom telemetry contains event/metric names only; it has no personal data, secret, resource identifier, request body, or storage metadata.

## Consequences

- Operators can query failures and core business activity in Application Insights without changing HTTP contracts.
- Key Vault access and deployment identity permissions are required before Live telemetry begins.
- Application Insights sampling and retention should be reviewed in Azure to manage ingestion cost.
