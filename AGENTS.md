# AGENTS.md — Underground Music Database

> Thin index. Read the linked file for detail only when the task needs it.

## Product context
Web platform to catalog underground music (Punk, Crust, Grindcore, Powerviolence, D-Beat).
Public browsing + community proposals + moderation + band claims + trust states.
Full scope: `docs/functional/overview.md`.

## Tech stack
- Backend: .NET / ASP.NET Core Web API, EF Core, SQL Server, JWT, Clean Architecture.
- Frontend: Angular, TypeScript, Reactive Forms, guards, interceptors.
- Storage: local files now, Azure Blob later via `IFileStorageService`.
- Monorepo: `backend/`, `frontend/`, `docs/`, `skills/`.

## Golden rules (non-negotiable)
1. **SDD first**: no code without a spec in `docs/specs/`. If missing/unclear, write the spec and pause.
2. **TDD always**: write the failing test before the implementation.
3. **Every task updates**: spec (if behavior changed) + tests + docs. A task is not done otherwise.
4. Respect Clean Architecture dependency direction. Domain depends on nothing.
5. Do not overengineer. This is a solo MVP with a hard deadline.

## Architecture rules
- Layers & responsibilities: `docs/technical/architecture.md`.
- Domain: entities/enums/rules only, no EF/HTTP.
- Application: use cases/DTOs/validators + interfaces.
- Infrastructure: EF, repos, storage, JWT.
- Api: thin controllers only.

## SOLID
Single responsibility per class/component; depend on abstractions; keep components/controllers thin.

## Backend rules → `.github/instructions/backend.instructions.md`
## Frontend rules → `.github/instructions/frontend.instructions.md`
## Testing rules → `docs/testing/guidelines.md`

## Naming conventions
- C#: PascalCase types, `IInterface`, tests `Scenario_Should_Result`.
- Angular: `kebab-case.type.ts`, PascalCase classes, one class per file.

## Git workflow
- Branch per feature: `feat/<spec-id>-<slug>`.
- Conventional commits: `feat:`, `test:`, `docs:`, `refactor:`, `fix:`.
- One feature = spec + tests + code + docs in the same PR.

## Commands
- Backend: `dotnet build`, `dotnet test`, `dotnet ef migrations add <Name>`.
- Frontend: `npm start`, `npm test`.

## What AI agents MUST NOT do
- Do not generate the whole app at once.
- Do not add libraries not in the stack (no MediatR/AutoMapper unless asked).
- Do not put business logic in controllers or Angular components.
- Do not delete/skip tests or lower coverage to pass CI.
- Do not implement Azure Blob Storage for the MVP.

## How to add a new feature
1. Create/update `docs/specs/NNN-name.md`.
2. Derive acceptance + unit tests.
3. Implement via TDD.
4. Update docs/API contract.
5. Confirm spec ✔ tests ✔ docs ✔.

## Skills index → `skills/`
- `skills/backend-dotnet-clean-architecture/SKILL.md`
- `skills/frontend-angular/SKILL.md`
- `skills/tdd-sdd-workflow/SKILL.md`
- `skills/security-review/SKILL.md`
- `skills/documentation-maintenance/SKILL.md`