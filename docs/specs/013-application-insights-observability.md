# Feature: Application Insights observability

## Functional goal

Provide operational telemetry for the API so administrators can identify failures, performance issues, dependency failures, and key successful business operations without collecting secrets or personal data.

## User story

As an operator, I want to observe the live API in Azure Application Insights so that I can respond to failures and understand whether users and administrators can complete important actions.

## Acceptance criteria (Given/When/Then checkboxes)

- [x] Given the API has an Application Insights connection string supplied by configuration, when it starts, then it sends request, dependency, exception, and `ILogger` telemetry to Application Insights.
- [x] Given an unhandled exception occurs while processing a request, when it is rethrown to ASP.NET Core, then it is logged with the exception and request context without request-body or user data.
- [x] Given a visitor registers successfully, when the API returns 201, then a `UserRegistered` business event and a `UsersRegistered` metric are tracked without personal data.
- [x] Given a user logs in successfully, when the API returns 200, then a `UserLoginSucceeded` business event and a `SuccessfulLogins` metric are tracked without personal data.
- [x] Given an administrator completes a catalog write operation, when the API returns a success response, then its corresponding business event and metric are tracked without resource IDs, names, or uploaded-file data.
- [x] Given the live API starts, when configuration is loaded, then the Application Insights connection string is read from Azure Key Vault rather than a committed configuration file.

## API contract

No HTTP endpoint, request, or response contract changes.

## Telemetry contract

Automatic telemetry: requests, failed requests, response duration, dependencies, unhandled exceptions, and `Warning`/`Error` logs.

Custom events and metrics:

| Event | Metric |
|---|---|
| `UserRegistered` | `UsersRegistered` |
| `UserLoginSucceeded` | `SuccessfulLogins` |
| `BandCreated` | `BandsCreated` |
| `BandUpdated` | `BandsUpdated` |
| `BandDeleted` | `BandsDeleted` |
| `MemberCreated` | `MembersCreated` |
| `MemberUpdated` | `MembersUpdated` |
| `MemberDeleted` | `MembersDeleted` |
| `ReleaseCreated` | `ReleasesCreated` |
| `ReleaseUpdated` | `ReleasesUpdated` |
| `ReleaseTracksUpdated` | `ReleaseTrackListsUpdated` |
| `BandImageConfirmed` | `BandImagesConfirmed` |

## Security and privacy rules

- Telemetry must not include emails, usernames, passwords, JWTs, connection strings, instrumentation keys, entity IDs, request bodies, uploaded-file names, storage paths, or signed URLs.
- `ApplicationInsights:ConnectionString` is a secret. It is supplied through .NET User Secrets locally and Azure Key Vault in Live.
- The committed value is only the placeholder `SET_VIA_KEYVAULT_OR_USER_SECRETS`.

## Test scenarios

- Unit: tracking a business operation sends exactly its event and metric names with no custom properties.
- Unit: an unsupported business operation cannot be tracked accidentally.
- Integration: existing API tests continue to start without an Application Insights secret or network telemetry.

## Out of scope

- Client-side telemetry.
- Availability tests, Azure alert rules, workbooks, dashboards, retention settings, and cost alerts. These are configured in Azure after deployment.
