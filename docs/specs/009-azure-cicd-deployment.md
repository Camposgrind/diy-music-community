# Feature: Azure CI/CD deployment for the monorepo

## Functional goal

Deploy the Angular frontend to the existing Azure Static Web App and the ASP.NET
Core API to the existing Azure App Service after a successful push to `master`.
The application must use Azure SQL Database, Azure Blob Storage, and Azure Key
Vault without committing secrets to the repository.

## User story

As the project maintainer, I want each independently changed application in the
monorepo to be tested and deployed automatically so that production is updated
only by validated code and secrets remain outside GitHub and source control.

## Acceptance criteria

- [ ] Given a push to `master` changes files under `frontend/`, when frontend
  tests and the production build succeed, then the Static Web App
  `diymusiccommunity-web` is deployed.
- [ ] Given a push to `master` changes files under `backend/`, when all .NET
  tests and the publish build succeed, then the App Service
  `diymusiccommunity-api` is deployed.
- [ ] Given tests or a build fail, when the respective workflow runs, then no
  deployment action is executed.
- [ ] Given a pull request changes either application, when its workflow runs,
  then the respective tests run without deploying production.
- [ ] Given the API deployment workflow authenticates to Azure, when it runs,
  then it uses GitHub OpenID Connect and short-lived credentials rather than a
  publish profile or client secret.
- [ ] Given the frontend deployment workflow runs, when it needs its deployment
  credential, then it reads the Azure Static Web Apps token from a GitHub secret.
- [ ] Given the API starts in Azure, when it reads production configuration,
  then secrets are read from Key Vault using the App Service managed identity.
- [ ] Given a production browser calls the API, when CORS is evaluated, then
  only the Static Web App production hostname is allowed.
- [ ] Given Azure SQL, Blob Storage, and Key Vault are provisioned, when the API
  accesses them, then least-privilege network and identity controls are used.

## API contract

No API endpoint contract changes. The frontend production API base URL is the
public HTTPS URL of `diymusiccommunity-api`, with `/api` appended.

## Domain rules

None.

## Permission rules

- The GitHub deployment identity has `Website Contributor` scoped to the App
  Service only.
- The App Service system-assigned managed identity has `Key Vault Secrets User`
  scoped to the Key Vault.
- The App Service uses a dedicated SQL login with access only to this database.
- Blob access is restricted to the API identity or to the currently implemented
  storage credential held only in Key Vault; no storage key is committed.

## Validation rules

- Workflow files must contain no resource credentials, connection strings,
  tokens, storage keys, or publish profiles.
- A production workflow deploys only from `master` after the relevant tests pass.

## Test scenarios (unit / integration)

- Frontend CI runs `npm ci`, `npm run test:run`, and `npm run build -- --configuration live`.
- Backend CI runs `dotnet restore`, `dotnet test`, and `dotnet publish`.
- Workflow triggers are limited to their respective folder plus their workflow file.

## Out of scope

- Infrastructure-as-code provisioning of the existing Azure resources.
- Preview environments for pull requests.
- Private endpoints and VNet integration, which are unavailable on the current
  App Service Free (F1) plan.
