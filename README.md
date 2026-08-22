# DIY Music Community

A web application for documenting and exploring underground and DIY music bands: Punk, Crust, Grindcore, Powerviolence, and D-Beat. The catalogue is public, while its curation is restricted to accounts with the `Admin` role.

Developed as a master's degree project, the application uses Clean Architecture in the backend, Angular standalone components in the frontend, and automated tests for its core flows.

## Contents

- [Features](#features)
- [Technology stack](#technology-stack)
- [Architecture and project structure](#architecture-and-project-structure)
- [Requirements](#requirements)
- [Local installation and execution](#local-installation-and-execution)
- [Testing and quality](#testing-and-quality)
- [API and technical documentation](#api-and-technical-documentation)

## Features

### Public catalogue

- Paginated band listing with filters by name, country, genre, and status.
- Band detail pages with general information, discography, and lineup.
- Release detail pages with ordered track listings.
- Responsive interface and global loading states in the web client.

### Catalogue administration

- JWT-based sign-in for administrator accounts.
- Create, edit, and delete bands.
- Manage current members, past members, and the last known lineup of split-up bands.
- Manage releases, formats, and track lists, including track reordering and deletion.
- Temporary upload and confirmation flow for band and release images. Permanent files are stored in Azure Blob Storage and the application generates read-only URLs on demand.

Visitors can browse the catalogue without authentication. All data-changing operations are protected by the `Admin` role.

## Technology stack

| Area | Technologies |
|---|---|
| Frontend | Angular 22, TypeScript, SCSS, RxJS, reactive forms, and standalone components |
| Backend | .NET 10, ASP.NET Core Web API, and C# |
| Data persistence | Entity Framework Core 10 and SQL Server |
| Identity | ASP.NET Core Identity and JWT Bearer authentication |
| File storage | Azure Blob Storage and short-lived local files during uploads |
| API | OpenAPI/Swagger |
| Observability | Application Insights (configurable) |
| Backend testing | xUnit, Moq, and integration tests |
| Frontend testing | Vitest and Angular testing utilities |
| Delivery | GitHub Actions, Azure App Service, and Azure Static Web Apps |

## Architecture and project structure

This repository is a monorepo with separate server and client applications.

```text
.
├── backend/
│   ├── src/
│   │   ├── DiyMusicCommunity.Domain/          # Domain entities, rules, and contracts
│   │   ├── DiyMusicCommunity.Application/     # Use cases, DTOs, and validation
│   │   ├── DiyMusicCommunity.Infrastructure/  # EF Core, SQL Server, Identity, and Blob Storage
│   │   └── DiyMusicCommunity.Api/             # HTTP controllers, JWT, Swagger, and composition root
│   └── tests/                                 # Unit and integration tests
├── frontend/
│   └── src/app/
│       ├── core/                              # Auth, guards, interceptors, and global services
│       ├── shared/                            # Reusable components and pipes
│       ├── infrastructure/api/                # Typed HTTP clients
│       └── features/                          # Home, bands, releases, and administration screens
├── docs/
│   ├── specs/                                 # Verifiable functional specifications
│   ├── technical/                             # Data model and OpenAPI contract
│   ├── adr/                                   # Architecture decision records
│   ├── functional/                            # Functional overview
│   ├── operations/                            # Azure and observability operations
│   └── testing/                               # Testing conventions
└── .github/workflows/                         # Continuous integration and deployment
```

The backend follows Clean Architecture: `Domain` has no dependencies on other layers; `Application` depends only on `Domain`; `Infrastructure` implements the contracts; and `Api` is the HTTP entry point. Controllers are intentionally lightweight and delegate behaviour to use cases.

## Requirements

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- A Node.js version compatible with Angular 22 and npm 10.9 or later
- SQL Server 2022+ or SQL Server LocalDB for local development
- An Azure Storage account only when testing image uploads

The repository contains no credentials. Development secrets are configured with *user secrets*, while deployments use Azure Key Vault.

## Local installation and execution

### 1. Clone and install dependencies

```powershell
git clone <REPOSITORY-URL>
cd diy-music-community
dotnet restore backend/DiyMusicCommunity.slnx
cd frontend
npm ci
cd ..
```

### 2. Configure the backend

From the repository root, set the API user secrets. Replace the example values with your local configuration; the JWT key must be at least 32 characters long.

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=DiyMusicCommunityDb;Trusted_Connection=True;TrustServerCertificate=True" --project backend/src/DiyMusicCommunity.Api
dotnet user-secrets set "Jwt:Key" "ALocalDevelopmentKeyWithAtLeast32Characters" --project backend/src/DiyMusicCommunity.Api
dotnet user-secrets set "Seed:AdminEmail" "admin@example.com" --project backend/src/DiyMusicCommunity.Api
```

To test image uploads, also configure the storage account:

```powershell
dotnet user-secrets set "AzureStorage:ConnectionString" "<AZURE_STORAGE_CONNECTION_STRING>" --project backend/src/DiyMusicCommunity.Api
dotnet user-secrets set "AzureStorage:ContainerName" "diy-music-community" --project backend/src/DiyMusicCommunity.Api
```

Do not add secrets to `appsettings.json`, the repository, or public documentation.

### 3. Run the API and database

The API applies pending migrations and creates the required Identity roles when it starts.

```powershell
dotnet run --project backend/src/DiyMusicCommunity.Api
```

In development, the API is available at:

- HTTPS API: `https://localhost:7294/api`
- HTTP API: `http://localhost:5002/api`
- Swagger: `https://localhost:7294/swagger`

To create the first administrator, configure `Seed:AdminEmail`, register that email through `POST /api/auth/register` in Swagger, and restart the API. The startup seeder will assign the `Admin` role. The public interface does not expose registration; the administration sign-in page is available at `/admin/login`.

### 4. Run the frontend

In a second terminal:

```powershell
cd frontend
npm start
```

Open `http://localhost:4200`. The development proxy forwards `/api` requests to `https://localhost:7294`.

### Production build

```powershell
dotnet build backend/DiyMusicCommunity.slnx --no-restore
cd frontend
npm run build
```

## Testing and quality

Run the tests before submitting or deploying changes:

```powershell
dotnet test backend/DiyMusicCommunity.slnx
cd frontend
npm run test:run
```

The backend includes domain, use-case, and API integration tests. The frontend tests components, HTTP services, route guards, interceptors, and form validation with Vitest.

## API and technical documentation

- Interactive API documentation is available through Swagger whenever the backend runs outside Production.
- The reference contract is in [docs/technical/openapi.md](docs/technical/openapi.md).
- The data model is documented in [docs/technical/erd.md](docs/technical/erd.md).
- User stories and acceptance criteria are in [docs/specs](docs/specs).
- Key design decisions are recorded in [docs/adr](docs/adr).
- Azure Blob Storage and Application Insights operations are explained in [docs/operations](docs/operations).

## License

This project is distributed under the license specified in [LICENSE](LICENSE).
