# Copilot Global Instructions — DIY Music Community

## Product
Web platform to catalog underground/DIY music scenes (Punk, Crust, Grindcore, Powerviolence, D-Beat).
Features: public browsing, community proposals, moderation, band claims, trust states.

## Stack
- **Backend:** .NET 8, Clean Architecture (Domain / Application / Infrastructure / Api), EF Core (SQLite dev / Postgres prod), FluentValidation, JWT, Result pattern.
- **Frontend:** Angular 17+, standalone components, Reactive Forms, `dmc-` selector prefix.
- **Tests:** xUnit + Moq (backend), Vitest (frontend). `dotnet test` / `npm test`.
- **Secrets:** Azure Key Vault (staging/prod) + .NET User Secrets (local dev). Never hardcode secrets.

## Architecture rules (NEVER break these)
- Dependency rule: `Domain` ← `Application` ← `Infrastructure` ← `Api`. No reverse references.
- `Domain` must NOT contain EF Core attributes, DTOs, HTTP concerns, or DI.
- `Application` must NOT contain EF Core, SQL, or controllers.
- `Api` controllers must NOT contain business logic.
- Use the **Result pattern** (`Result<T>` + `Error`) for expected failures; throw exceptions only for exceptional cases.
- No MediatR. No AutoMapper.

## Workflow (mandatory, every task)
1. Spec exists in `docs/specs/NNN-feature-name.md`? If not, create/update it first.
2. Write failing tests derived from the spec (TDD).
3. Implement to make tests pass.
4. Run `dotnet test` and `npm test` — both must be green.
5. Update spec + docs in the same PR.

## Naming
- Git branches: `feat/<spec-id>-<slug>` (e.g. `feat/001-public-band-browsing`)
- Commits: Conventional Commits (`feat:`, `test:`, `docs:`, `refactor:`, `fix:`)
- Backend test names: `Scenario_Should_Result`
- Frontend test names: `it('should ...')`
- Angular selectors: `dmc-` prefix; files: `kebab-case.type.ts`

## Must NOT do
- Delete tests or lower coverage thresholds to pass CI.
- Add EF Core / SQL to Domain or Application.
- Add business logic to Api controllers.
- Skip writing or updating the spec.
- Use MediatR, AutoMapper, or Nx.
- Build Azure Blob Storage for the MVP (use `IFileStorageService` + `LocalFileStorageService`).
- Add refresh tokens, email verification, or password reset.
- **Hardcode any secret, password, connection string, or API key in any file.**
- Commit `appsettings.Local.json`, `.env`, or any file containing real secret values.
- Put secrets in `environment.ts` or any frontend file.

## Key decisions (do not revisit)
- Scope: browsing, band detail, proposals, moderation, claims, trust states.
- JWT only; `sub` + `role` claims.
- Band claim = resource-based check (`IBandAccessService`), not a global role.
- Strings for Country / Label / Formats — no extra entities.
- Coverage: ~60% overall, ~85% Domain + Application.
- Skip E2E if short on time.
- Secrets: Azure Key Vault in deployed envs; .NET User Secrets locally. See `docs/adr/001-secrets-management.md`.

## Skills index (`skills/`)
| Skill | File |
|---|---|
| Backend Clean Architecture | `skills/backend-dotnet-clean-architecture/SKILL.md` |
| Frontend Angular | `skills/frontend-angular/SKILL.md` |
| TDD + SDD workflow | `skills/tdd-sdd-workflow/SKILL.md` |
| Security review | `skills/security-review/SKILL.md` |
| Documentation maintenance | `skills/documentation-maintenance/SKILL.md` |