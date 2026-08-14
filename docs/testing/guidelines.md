# Testing Guidelines — DIY Music Community

## Philosophy
- Tests are derived from specs in `docs/specs/`. If behavior is unclear, check the spec first.
- Write the **failing test first** (TDD), then implement.
- Never delete tests or lower coverage thresholds to pass CI.

## Coverage targets
| Scope | Target |
|---|---|
| Domain + Application | ~85% |
| Overall | ~60% |
| E2E | Optional — skip if short on time |

Do not chase 100%. Prioritize correctness of critical flows over raw coverage numbers.

## Backend (xUnit + Moq)

### Test project layout
```
tests/
├── DiyMusicCommunity.Domain.Tests/
├── DiyMusicCommunity.Application.Tests/
└── DiyMusicCommunity.Api.IntegrationTests/
```

### Naming convention
`Scenario_Should_Result`
```csharp
AdminCreatesBand_Should_ReturnBand()
NonAdminCreatesBand_Should_Return403()
GetBands_Should_ReturnMatchingPage()
```

### Layer guidance
| Layer | Test type | Tools | Mock? |
|---|---|---|---|
| Domain | Unit | xUnit | Nothing — pure logic |
| Application | Unit | xUnit + Moq | Repository interfaces, `ICurrentUser` |
| Api | Integration | xUnit + `WebApplicationFactory` + SQLite in-memory | Nothing |

### Must-have test scenarios
1. Admin creates a band → the band is persisted and returned.
2. Admin updates a band → the amended catalog data is persisted and returned.
3. Band creation or update called with a non-Admin role → `403 Forbidden`.
4. Protected catalog-management endpoint called without a token → `401 Unauthorized`.
5. Public band list → matching results are paginated.

### Run command
```bash
cd backend
dotnet test
```

## Frontend (Vitest)

### Setup
Frontend tests run with **Vitest**. Config lives in `frontend/vitest.config.ts`.
Do not configure Karma or Jasmine — they are not used in this project.

Angular's `TestBed` and `HttpTestingController` work normally with Vitest via
`@analogjs/vitest-angular` (Angular 17) or the built-in Vitest support (Angular 18+).

### Test file co-location
```
features/bands/band-list/
├── band-list.component.ts
├── band-list.component.html
└── band-list.component.spec.ts
```

### Naming convention
```typescript
it('should display band list when bands are returned from the service')
it('should show validation error when band name is empty')
it('should redirect to login when user is unauthenticated')
it('should disable submit button while request is pending')
```

### What to test
- Component logic: state transitions, method calls, template bindings.
- Guards: `auth.guard`, `role.guard` (including rejection of non-Admin users from catalog-management routes).
- Form validation rules (required, minLength, pattern).
- HTTP services via `HttpTestingController`.

### What NOT to test
- Third-party library internals.
- Private methods not reflected in the template or public API.
- Implementation details that could change without affecting behavior.

### Run commands
```bash
cd frontend
npm test                  # run all tests (watch mode)
npm run test:run          # run once (CI mode)
npm run test:coverage     # with coverage report
```

## CI requirements
Both `dotnet test` and `npm test` must be **green** before any PR is merged.
Red CI = do not merge. Fix the code, not the test.
