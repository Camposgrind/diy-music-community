---
applyTo: "**/*.spec.ts,**/*Tests.cs"
---

# Test Instructions — DIY Music Community

## General rules
- **Never delete tests** or lower coverage thresholds to make CI pass.
- Write the **failing test first**, then implement.
- Tests are derived from `docs/specs/` — if behavior is unclear, check the spec first.

## Backend (xUnit + Moq)

### Naming
`Scenario_Should_Result`
```csharp
// Good
ApprovePendingProposal_Should_CreatePublishedBand()
RejectProposalWithoutReason_Should_ReturnFailure()
SubmitDuplicateClaim_Should_ReturnConflictError()
```

### Layer targets
| Layer | Test type | What to mock |
|---|---|---|
| Domain | Unit | Nothing — test pure logic |
| Application | Unit | Repository interfaces, `ICurrentUser` |
| Api | Integration | Nothing — `WebApplicationFactory` + SQLite in-memory |

### Must-have scenarios
- Proposal approval → Band created with `CommunityCreated` trust status.
- Claim approval → Band `TrustStatus = Claimed`, `IsClaimed = true`.
- Role access → 403 for unauthorized roles.
- Duplicate pending claim → conflict error.
- Public band list filtering → blocked bands excluded.

### Coverage targets
- Domain + Application: ~85%.
- Overall: ~60%.
- Do not chase 100%.

## Frontend (Vitest)

### Setup
Frontend tests run with **Vitest** (`@analogjs/vitest-angular` or Angular's built-in Vitest integration).
Config lives in `frontend/vitest.config.ts`. Do not configure Karma or Jasmine.

### Naming convention
```typescript
it('should display band list when bands are returned')
it('should show validation error when name is empty')
it('should redirect to login when unauthenticated')
```

### What to test
- Component logic (state changes, method calls).
- Guards (`auth.guard`, `role.guard`, `claim.guard`).
- Form validation rules.
- HTTP services via `HttpTestingController` (Angular's test utilities work normally with Vitest).

### What NOT to test
- Third-party library internals.
- Implementation details (private methods, internal state not reflected in the template).

## Commands
```bash
# Backend
dotnet test

# Frontend
npm test        # runs vitest
npm run test:coverage   # with coverage report
```

Both commands must be green before merging any PR.