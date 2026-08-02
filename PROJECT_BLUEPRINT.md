# DIY Music Community — Architecture & AI Workflow Blueprint

A pragmatic, MVP-focused Clean Architecture + SOLID + TDD + SDD blueprint for a solo
developer working with AI agents under a tight deadline (Aug 2 → Aug 23).

**Product:** A web platform to catalog underground/DIY music scenes (Punk, Crust,
Grindcore, Powerviolence, D-Beat). Public browsing + community proposals + moderation
+ band claims + trust states.

---

## Naming conventions

| Thing | Name |
|---|---|
| Repository | `diy-music-community` |
| .NET solution | `DiyMusicCommunity.sln` |
| Backend projects | `DiyMusicCommunity.Domain` / `.Application` / `.Infrastructure` / `.Api` |
| Root namespace | `DiyMusicCommunity` |
| Angular app / npm package | `diy-music-community-web` |
| Angular selector prefix | `dmc-` (e.g. `dmc-band-list`) |
| Database | `DiyMusicCommunityDb` |
| Git branch | `feat/<spec-id>-<slug>` (e.g. `feat/001-public-band-browsing`) |
| Commits | Conventional commits (`feat:`, `test:`, `docs:`, `refactor:`, `fix:`) |

---

# 1. Monorepo Architecture

```text
diy-music-community/
├── README.md
├── AGENTS.md
├── PROJECT_BLUEPRINT.md
├── .editorconfig
├── .gitignore
├── LICENSE
│
├── .github/
│   ├── copilot-instructions.md
│   ├── instructions/
│   │   ├── backend.instructions.md      # applyTo: "backend/**/*.cs"
│   │   ├── frontend.instructions.md     # applyTo: "frontend/**/*.ts"
│   │   └── tests.instructions.md        # applyTo: "**/*.spec.ts, **/*Tests.cs"
│   └── workflows/
│       ├── backend-ci.yml
│       └── frontend-ci.yml
│
├── backend/
│   ├── DiyMusicCommunity.sln
│   ├── src/
│   │   ├── DiyMusicCommunity.Domain/
│   │   ├── DiyMusicCommunity.Application/
│   │   ├── DiyMusicCommunity.Infrastructure/
│   │   └── DiyMusicCommunity.Api/
│   └── tests/
│       ├── DiyMusicCommunity.Domain.Tests/
│       ├── DiyMusicCommunity.Application.Tests/
│       └── DiyMusicCommunity.Api.IntegrationTests/
│
├── frontend/                            # Angular app: diy-music-community-web
│   ├── angular.json
│   ├── package.json
│   └── src/app/
│       ├── core/
│       ├── shared/
│       └── features/
│
├── docs/
│   ├── functional/
│   ├── technical/
│   ├── adr/
│   ├── specs/
│   ├── api/
│   └── testing/
│
├── skills/
│   ├── backend-dotnet-clean-architecture/SKILL.md
│   ├── frontend-angular/SKILL.md
│   ├── tdd-sdd-workflow/SKILL.md
│   ├── security-review/SKILL.md
│   └── documentation-maintenance/SKILL.md
│
└── scripts/
    ├── setup.sh / setup.ps1
    ├── db-migrate.sh
    └── seed.sh
```

| Folder | Purpose |
|---|---|
| `.github/copilot-instructions.md` | Repo-wide rules auto-loaded by Copilot. |
| `.github/instructions/*` | Path-scoped rules via `applyTo` frontmatter. |
| `backend/` | .NET Clean Architecture layers + test projects. |
| `frontend/` | Angular app (`diy-music-community-web`). |
| `docs/` | Human + AI docs split by concern. |
| `docs/adr/` | Records of *why* decisions were made. |
| `docs/specs/` | SDD specs (source of truth for behavior). |
| `skills/` | Reusable AI task playbooks (portable Markdown). |
| `scripts/` | Reproducible setup automation. |

**Monorepo decision:** single repo, no Nx/Turborepo. Folder separation + two CI jobs is enough for a solo MVP.

---

# 2. Backend Architecture (Clean Architecture)

Dependency rule: dependencies point inward. `Domain` depends on nothing.

```text
DiyMusicCommunity.Domain          ← no dependencies
DiyMusicCommunity.Application     ← depends on Domain
DiyMusicCommunity.Infrastructure  ← depends on Application + Domain
DiyMusicCommunity.Api             ← depends on Application (+ Infrastructure via DI only)
```

### Domain
**Contains:** Entities, enums, value objects, domain rules, domain exceptions, repository *interfaces*.
**Must NOT contain:** EF Core attributes, DTOs, HTTP concerns, DI.

```text
Domain/
├── Entities/     Band, Release, Track, BandMember, BandProposal, BandClaim, ModerationAction, User
├── Enums/        BandStatus, TrustStatus, ProposalStatus, ClaimStatus, ClaimType, UserRole
├── ValueObjects/ (optional) SourceUrl, EvidenceUrl
├── Exceptions/   DomainException, InvalidProposalTransitionException
└── Abstractions/ IBandRepository, IProposalRepository, IClaimRepository, IUnitOfWork
```

### Application
**Contains:** Use cases, DTOs, validators (FluentValidation), mapping, interfaces
(`IFileStorageService`, `IJwtTokenService`, `ICurrentUser`).
**Must NOT contain:** EF Core, SQL, controllers.

```text
Application/
├── Bands/      GetBandsQuery, GetBandByIdQuery, dtos, validators
├── Proposals/  CreateProposalCommand, ApproveProposalCommand, RejectProposalCommand
├── Claims/     SubmitClaimCommand, ApproveClaimCommand, RejectClaimCommand
├── Auth/       RegisterCommand, LoginCommand
├── Common/     Result<T>, Error, pagination
└── Abstractions/ IFileStorageService, IJwtTokenService, ICurrentUser
```

> Use plain use-case classes (`Handle(request)`). No MediatR unless you already know it.

### Infrastructure
**Contains:** `AppDbContext`, EF configurations, repositories, migrations, seed data,
`LocalFileStorageService`, JWT implementation, password hashing.
**Must NOT contain:** business rules, validation logic.

```text
Infrastructure/
├── Persistence/  AppDbContext, Configurations/, Migrations/, Seed/
├── Repositories/ BandRepository, ProposalRepository, ClaimRepository
├── Storage/      LocalFileStorageService  (AzureBlobStorageService later)
├── Auth/         JwtTokenService, PasswordHasher
└── DependencyInjection.cs
```

### Api
**Contains:** thin controllers, middleware, exception filter, DI composition root,
`Program.cs`, auth setup, Swagger.
**Must NOT contain:** business logic.

```text
Api/
├── Controllers/  Auth, Bands, Releases, BandProposals, BandClaims, Moderation
├── Middleware/   ExceptionHandlingMiddleware
├── Program.cs
└── appsettings.json
```

### Pragmatic recommendations
- **Result pattern** (`Result<T>` + `Error`) for expected failures; exceptions only for exceptional cases.
- **JWT**: contains `sub` (userId) + `role`. No refresh tokens for MVP.
- **Role auth**: `[Authorize(Roles = "Moderator,Admin")]`.
- **Band claim auth**: NOT a role — resource-based check (see §6).
- **EF Core**: code-first, one `IEntityTypeConfiguration` per entity.
- **Seed**: idempotent, Development only (12 bands, admin + moderator users).
- **Storage**: `IFileStorageService` now (`LocalFileStorageService`); Azure Blob later. Do not build Azure for the MVP.

---

# 3. Frontend Architecture (Angular)

Angular selector prefix: `dmc-`.

```text
src/app/
├── core/
│   ├── auth/     auth.service.ts, auth.guard.ts, role.guard.ts, claim.guard.ts
│   ├── http/     auth.interceptor.ts, error.interceptor.ts
│   ├── models/   band.model.ts, proposal.model.ts, claim.model.ts, user.model.ts
│   └── services/ api base, notification service
│
├── shared/
│   ├── components/ trust-badge, loading-spinner, empty-state, pagination
│   ├── pipes/
│   └── directives/
│
└── features/
    ├── home/
    ├── bands/       band-list (page), band-detail (page), band-card (presentational)
    ├── releases/    release-detail (page), tracklist (presentational)
    ├── auth/        login (page), register (page)
    ├── proposals/   propose-band (page), my-proposals (page)
    ├── moderation/  moderation-dashboard, proposal-review, claim-review
    └── claims/      claim-band (page/dialog)
```

| Folder | Purpose |
|---|---|
| `core/` | App-wide singletons: auth, interceptors, guards, models. |
| `shared/` | Stateless reusable presentational components/pipes. No API calls. |
| `features/*` | Self-contained features: pages + local components + feature service. |

### Frontend rules
- **Avoid large components:** >~150 lines or >1 responsibility → split.
- **Pages (smart) vs presentational (dumb):** dumb components use `@Input()/@Output()` only, no services.
- **Reactive Forms only.**
- **API errors:** centralized `error.interceptor.ts` → toasts.
- **Auth state:** `AuthService` exposes current-user signal/BehaviorSubject; token in `localStorage`, attached by interceptor.
- **Naming:** `kebab-case.type.ts`, PascalCase classes, one class per file, `dmc-` selectors.

---

# 4. Initial Domain Model

Country/label/formats are strings for the MVP.

### User
- Auth identity + role.
- `Id, Email, PasswordHash, DisplayName, Role (User|Moderator|Admin), CreatedAt`.
- 1→N Proposals, 1→N Claims. Email unique; default role `User`.
- Tests: duplicate email rejected; new user gets `User`.

### Band
- Core catalog entity.
- `Id, Name, Country, Location, GenreId, Status (Active|SplitUp|OnHold), FormationYear, Description, TrustStatus (CommunityCreated|ClaimPending|Claimed|Blocked), IsClaimed, CreatedAt, UpdatedAt`.
- N→1 Genre, 1→N Releases, 1→N BandMembers, 1→N Claims.
- Rules: `IsClaimed` true only on approved claim; trust transitions constrained.
- Tests: approving claim → Claimed + IsClaimed; blocked excluded from list.

### Genre
- `Id, Name`. One primary genre per band for MVP.

### Release
- `Id, BandId, Title, ReleaseType (Demo|EP|Album|Split|Compilation), ReleaseDate, Year, LabelText, FormatsText, CoverImageUrl`.
- N→1 Band, 1→N Tracks. Tests: belongs to band; tracklist ordered.

### Track
- `Id, ReleaseId, Title, TrackNumber`. `TrackNumber` unique per release.

### BandMember
- `Id, BandId, Name, Instrument, StartYear, EndYear, IsCurrent, AlsoInBandsText`.
- Rule: `EndYear` set → `IsCurrent=false`. Tests: current vs past separated.

### BandProposal
- `Id, Name, Country, Location, GenreId, Status, FormationYear, Description, SourceUrl, SubmittedByUserId, ReviewStatus (Pending|Approved|Rejected), CreatedAt, ReviewedAt, ReviewedByUserId, RejectionReason`.
- Rules: only Pending can transition; approval creates published Band (CommunityCreated); rejection needs reason.
- Tests: approve pending → creates band; re-approve → error; reject w/o reason → error.

### BandClaim
- `Id, BandId, UserId, ClaimType (CurrentMember|PastMember|Representative|Label|Other), Message, EvidenceUrl, Status (Pending|Approved|Rejected), CreatedAt, ReviewedAt, ReviewedByUserId, RejectionReason`.
- Rules: submit → band ClaimPending; approve → Claimed + IsClaimed; no two pending claims per (user, band).
- Tests: duplicate pending rejected; approval updates band.

### ModerationAction
- `Id, ModeratorId, ActionType, TargetId, Reason, CreatedAt`.
- Rule: created on every decision. Tests: one action per approve/reject.

---

# 5. REST API Design

Base `/api`; lists paginated; error envelope `{ error: { code, message } }`.
Status: 400 validation, 401 unauthenticated, 403 forbidden, 404 not found, 409 conflict.

### Auth
| Method | Route | Request | Response | Role | Errors |
|---|---|---|---|---|---|
| POST | `/api/auth/register` | `RegisterDto` | `AuthResponseDto` | Anon | 400,409 |
| POST | `/api/auth/login` | `LoginDto` | `AuthResponseDto` | Anon | 400,401 |

### Bands
| Method | Route | Response | Role | Errors |
|---|---|---|---|---|
| GET | `/api/bands` (name,genreId,country,status,page) | `PagedResult<BandListItemDto>` | Anon | 400 |
| GET | `/api/bands/{id}` | `BandDetailDto` | Anon | 404 |

### Releases / Genres
| Method | Route | Response | Role | Errors |
|---|---|---|---|---|
| GET | `/api/releases/{id}` | `ReleaseDetailDto` | Anon | 404 |
| GET | `/api/genres` | `GenreDto[]` | Anon | – |

### BandProposals
| Method | Route | Request | Response | Role | Errors |
|---|---|---|---|---|---|
| POST | `/api/band-proposals` | `CreateProposalDto` | `ProposalDto` | User | 400,401 |
| GET | `/api/me/band-proposals` | – | `ProposalDto[]` | User | 401 |

### BandClaims
| Method | Route | Request | Response | Role | Errors |
|---|---|---|---|---|---|
| POST | `/api/bands/{bandId}/claims` | `CreateClaimDto` | `ClaimDto` | User | 400,401,404,409 |

### Moderation
| Method | Route | Request | Response | Role | Errors |
|---|---|---|---|---|---|
| GET | `/api/moderation/band-proposals` | – | `ProposalDto[]` | Mod/Admin | 401,403 |
| POST | `/api/moderation/band-proposals/{id}/approve` | – | `BandDto` | Mod/Admin | 403,404,409 |
| POST | `/api/moderation/band-proposals/{id}/reject` | `RejectDto` | `ProposalDto` | Mod/Admin | 400,403,404 |
| GET | `/api/moderation/claims` | – | `ClaimDto[]` | Mod/Admin | 401,403 |
| POST | `/api/moderation/claims/{id}/approve` | – | `ClaimDto` | Mod/Admin | 403,404,409 |
| POST | `/api/moderation/claims/{id}/reject` | `RejectDto` | `ClaimDto` | Mod/Admin | 400,403,404 |

Every endpoint has a corresponding happy-path + auth/role test.

---

# 6. Permissions Strategy

| Actor | Can do |
|---|---|
| Anonymous | Browse, list/filter, view band/release detail, members. |
| Registered user | + login, propose band, view own proposals, submit claim. |
| Moderator | + view/approve/reject proposals and claims. |
| Admin | + full access during demo. |
| User with approved claim | Registered user + (future) edit their band. |

**Approved claim without a global role:** ownership is per-band, not global.
Use a resource-based check — "does an `Approved` BandClaim exist for `(userId, bandId)`?" —
via `IBandAccessService.CanEditBand(userId, bandId)` or an `IAuthorizationHandler`.
JWT holds only `User/Moderator/Admin`.

---

# 7. TDD Strategy

| Layer | What to test | Priority |
|---|---|---|
| Domain unit | Entity rules, transitions, invariants | Must |
| Application | Use cases w/ mocked repos | Must |
| Api integration | Happy path + auth/role (WebApplicationFactory + SQLite) | Must (critical flows) |
| Frontend unit | Component logic, guards, form validation | Should |
| Frontend service | HTTP services (HttpTestingController) | Should |
| E2E | Full demo flow | Optional — skip if short on time |

**Must test:** proposal approval → band creation; claim approval → trust change;
role access (403); duplicate claim prevention; public filtering.
**Naming:** backend `Scenario_Should_Result`; frontend `it('should ...')`.
**Commands:** `dotnet test` / `npm test`.
**Coverage:** ~60% overall, ~85% Domain + Application. Don't chase 100%.
**AI behavior:** failing test first; never delete tests or lower thresholds to pass CI.

---

# 8. SDD Strategy

Specs live in `docs/specs/NNN-feature-name.md` — the source of truth.

**Workflow:** write/update spec → derive tests → implement (TDD) → run tests → update docs → done.
**Guard:** if a spec is missing/unclear, create it and pause before coding.
**Behavior change:** edit the spec in the same PR; add an ADR if architectural.

### Spec template
```markdown
# Feature: <name>
## Functional goal
## User story
## Acceptance criteria (Given/When/Then checkboxes)
## API contract
## Domain rules
## Permission rules
## Validation rules
## Test scenarios (unit / integration)
## Out of scope
```

Full example specs for **public band browsing**, **band proposal workflow**, and
**band claim workflow** are in `docs/specs/001–003`.

---

# 9. Documentation Strategy

| Document | Audience | Contains |
|---|---|---|
| `README.md` | Humans | Prereqs, run backend/frontend, test users, features, layout. |
| `AGENTS.md` | AI agents | Thin index of rules + links. |
| `.github/copilot-instructions.md` | Copilot global | Always-on condensed rules. |
| `.github/instructions/*` | Copilot scoped | Path-specific rules. |
| `docs/functional/` | PM/defense | Product behavior in plain language. |
| `docs/technical/` | Devs | Architecture, layers, data model. |
| `docs/adr/` | Devs | One ADR per decision. |
| `docs/specs/` | Devs + AI | SDD specs. |
| `docs/api/` | Devs + AI | Endpoint contracts / Swagger link. |
| `docs/testing/` | Devs + AI | Conventions, coverage, commands. |
| `docs/technical/prompt-recipes.md` | You + AI | Ready-to-use prompts. |

---

# 10. AGENTS.md (see repo root file)

Thin index with product context, stack, golden rules (SDD-first, TDD-always, every task
updates spec+tests+docs), architecture rules, naming, git workflow, commands,
"must not do" list, how to add a feature, and skills index.

---

# 11. AI Skills

Include a root `skills/` folder (portable Markdown, understood by Copilot, Codex, Claude),
referenced from `AGENTS.md`.

| Skill | Purpose | Boundary |
|---|---|---|
| backend-dotnet-clean-architecture | Layered backend features | No EF in Domain; no logic in Api |
| frontend-angular | Angular features | No API logic in dumb components |
| tdd-sdd-workflow | Spec→test→code loop | Never code without a spec |
| security-review | Auth/authorization review | Never weaken auth to pass tests |
| documentation-maintenance | Keep docs/specs in sync | Don't invent behavior |

---

# 12. Prompt Recipes

Stored in `docs/technical/prompt-recipes.md`: backend entity (TDD), application service (TDD),
API endpoint (TDD), Angular feature page, Angular service, Reactive Form, moderation workflow,
update spec, refactor without behavior change, add tests, review vs Clean Architecture / SOLID / SDD.

---

# 13. Implementation Plan (Aug 2–23)

| Phase | Goal | Dates | Done criteria |
|---|---|---|---|
| 0 – Docs & AI | Guardrails | Aug 2 | AGENTS/specs/skills committed |
| 1 – Monorepo | Skeleton | Aug 2 | build + ng build green |
| 2 – Backend skeleton | Layers + EF | Aug 3 | First migration created |
| 3 – Domain + tests | Model + seed | Aug 3–4 | Domain tests green, DB seeded |
| 4 – Public API | Read endpoints | Aug 5 | Swagger works, tests green |
| 5 – Angular public | Public UI | Aug 6–9 | Anonymous flow end-to-end |
| 6 – Auth | Login/roles | Aug 10–12 | Protected routes enforced |
| 7 – Proposals | Contribution | Aug 13–15 | Propose + my-proposals works |
| 8 – Moderation | Curation | Aug 16 | Approved proposal appears public |
| 9 – Claims | Verification | Aug 17–20 | Approved claim shows "Claimed" |
| 10 – Polish | Ship | Aug 21–23 | Demo script runs clean |

If you slip, cut in this order: reports → images → history → advanced editing → real Blob Storage.

---

# 14. Recommended First Copilot Agent Prompts

Incremental, safe, TDD+SDD-driven (see the numbered list in your working notes / §14 of the
original blueprint). Never "build everything at once".

---

# Recommended Key Decisions (do not change)

1. Scope freeze: browsing, band detail, proposals, moderation, claims, trust states.
2. Clean Architecture, 4 layers, no MediatR/AutoMapper.
3. Result pattern over exceptions for expected failures.
4. JWT only — no refresh tokens / email verification / password reset.
5. Band claim = resource-based check, not a global role.
6. Strings for Country/Label/Formats; no Person/Label/Format entities yet.
7. Local file storage only; Azure Blob documented as future.
8. SDD + TDD mandatory, enforced by AGENTS.md.
9. Coverage ~60% overall, ~85% Domain/Application.
10. Skip E2E if short on time.
11. Solo-scale monorepo, no Nx.
12. Follow the phase calendar; cut scope in the documented order.