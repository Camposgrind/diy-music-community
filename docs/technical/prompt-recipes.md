# Prompt Recipes — DIY Music Community

Ready-to-use prompts for working with AI agents on this project.
Always supply the relevant spec (`docs/specs/NNN-*.md`) as context when using these prompts.

---

## Backend: New domain entity + tests
```
Using the spec at docs/specs/NNN-<name>.md and the rules in skills/backend-dotnet-clean-architecture/SKILL.md:

1. Create the `<Entity>` entity in `Domain/Entities/` with the properties and domain rules defined in the spec.
2. Create the `I<Entity>Repository` interface in `Domain/Abstractions/`.
3. Write domain unit tests in `DiyMusicCommunity.Domain.Tests` covering all invariants and transitions.
4. Do NOT add EF Core attributes or any infrastructure concern to the entity.
```

## Backend: New application use case + tests
```
Using the spec at docs/specs/NNN-<name>.md:

1. Create `<UseCase>Command` or `<UseCase>Query` + DTOs in `Application/<Feature>/`.
2. Add a FluentValidation validator.
3. Implement the use case with `Handle(request)` returning `Result<T>`.
4. Write application unit tests in `DiyMusicCommunity.Application.Tests` mocking all repository interfaces.
5. Cover success path, validation failure, and not-found / conflict cases.
```

## Backend: New API endpoint + integration tests
```
Using the spec at docs/specs/NNN-<name>.md:

1. Add/update the controller in `Api/Controllers/` — thin, no business logic.
2. Map `Result<T>` to the correct HTTP status codes per the spec.
3. Apply `[Authorize]` / `[Authorize(Roles = "...")]` as specified.
4. Write integration tests in `DiyMusicCommunity.Api.IntegrationTests` using WebApplicationFactory + SQLite covering:
   - Happy path (correct response body + status).
   - Auth failure (401 without token, 403 wrong role).
   - Validation failure (400).
   - Not found (404) if applicable.
```

## Frontend: New feature page
```
Using the spec at docs/specs/NNN-<name>.md and the rules in skills/frontend-angular/SKILL.md:

1. Create a smart page component in `features/<feature>/` with selector `dmc-<name>-page`.
2. Create any required dumb components (selector `dmc-<name>`), accepting data only via @Input() / @Output().
3. Create a feature service that calls the API and returns Observable<T>.
4. Register the route with appropriate guards per the spec.
5. Write component tests and HTTP service tests.
```

## Frontend: New Angular service (HTTP)
```
Create a service for the `<Feature>` feature:

1. Inject HttpClient; return typed Observables.
2. Base URL: `/api/<resource>`.
3. Write tests using HttpTestingController covering success, 400, 401, and 404 responses.
4. Handle errors via the existing error.interceptor — do not catch errors in the service.
```

## Frontend: Reactive Form with validation
```
Add a Reactive Form for <feature> following these rules:

1. Define the FormGroup in the component class (no template-driven).
2. Add validators per the spec's validation rules.
3. Display inline validation messages in the template.
4. Disable the submit button while the request is in flight.
5. Write tests that verify each validation rule triggers the correct error message.
```

## Administrator catalog workflow (backend + frontend)
```
Using the spec at docs/specs/NNN-<name>.md, implement administrator-only create/update management for bands:

Backend:
- Create and update use cases validate the request and persist the band.
- Controller endpoints use an `Admin` role guard.
- Integration tests cover the happy paths, 401 without a token, and 403 for a non-Admin role.

Frontend:
- Administrator-only create and edit pages.
- Forms show validation feedback and redirect or refresh after a successful save.
- Do not expose contribution, claim, or moderation screens.
```

## Update spec after behavior change
```
The behavior of <feature> has changed as follows: <describe change>.

1. Update docs/specs/NNN-<name>.md — acceptance criteria, domain rules, and validation rules sections.
2. Mark changed ACs with [ ] if they now need new tests, or update existing ones.
3. Add an ADR in docs/adr/ if this was an architectural decision.
4. List which existing tests need to be updated and why.
Do NOT change code — only update the spec and flag what needs to change.
```

## Refactor without behavior change
```
Refactor <file or feature> for <reason: readability / SOLID / layer violation>.

Rules:
- All existing tests must remain green after the refactor.
- Do not add new behavior.
- Do not change public API contracts.
- If you discover a layer violation, fix it and note it in the PR description.
Run dotnet test / npm test after each logical step.
```

## Add tests for existing code
```
Add tests for <class or feature> to reach ~85% coverage on Domain/Application.

1. Read the existing implementation and identify untested paths.
2. Check docs/specs/ to confirm the expected behavior.
3. Write tests following the naming convention (Scenario_Should_Result / it('should ...')).
4. Do not change the production code — only add tests.
5. If you find a bug while writing tests, note it but do not fix it in this PR.
```

## Review against Clean Architecture / SOLID / SDD
```
Review the code changes in this PR against:
1. Clean Architecture: no reverse dependencies, correct layer boundaries.
2. SOLID: single responsibility, open/closed, dependency inversion.
3. SDD: does the implementation match docs/specs/NNN-<name>.md?
4. TDD: are all new behaviors covered by tests?

Report: (a) violations found, (b) suggestions to fix them, (c) confirm if the spec is up to date.
```
