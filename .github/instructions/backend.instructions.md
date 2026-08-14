---
applyTo: "backend/**/*.cs"
---

# Backend Instructions — DIY Music Community

## Layer rules
| Layer | Allowed dependencies | Forbidden |
|---|---|---|
| `Domain` | None | EF Core, DTOs, HTTP, DI |
| `Application` | Domain only | EF Core, SQL, controllers |
| `Infrastructure` | Application + Domain | Business rules, validation logic |
| `Api` | Application (Infrastructure via DI only) | Business logic in controllers |

## Domain
- Entities in `Domain/Entities/`, enums in `Domain/Enums/`, value objects in `Domain/ValueObjects/`.
- Repository *interfaces* in `Domain/Abstractions/` (`IBandRepository`, `IUnitOfWork`).
- Domain exceptions in `Domain/Exceptions/` (`DomainException`).
- No EF Core attributes on domain entities.

## Application
- One folder per feature: `Bands/`, `Auth/`, `Common/`.
- Use plain use-case classes with a `Handle(request)` method — no MediatR.
- Use FluentValidation for all input validation.
- Return `Result<T>` for expected failures; never throw for business rule violations.
- Service interfaces (`IFileStorageService`, `IJwtTokenService`, `ICurrentUser`) live in `Application/Abstractions/`.

## Infrastructure
- One `IEntityTypeConfiguration<T>` per entity.
- Migrations in `Infrastructure/Persistence/Migrations/`.
- Seed data: idempotent, Development environment only (12 bands and an admin user).
- `LocalFileStorageService` implements `IFileStorageService` — no Azure for MVP.
- JWT implementation in `Infrastructure/Auth/JwtTokenService.cs`.

## Api
- Thin controllers: call use case → return HTTP result.
- `ExceptionHandlingMiddleware` catches unhandled exceptions → structured error response.
- Auth: `[Authorize(Roles = "Admin")]` for band creation and update endpoints.
- Error envelope: `{ "error": { "code": "...", "message": "..." } }`.

## Result pattern
```csharp
// Return from use cases
Result<BandDto>.Success(dto)
Result<BandDto>.Failure(Error.NotFound("Band.NotFound", "Band not found"))
```

## Naming
- Test method names: `Scenario_Should_Result` (e.g. `AdminCreatesBand_Should_ReturnBand`).
- One class per file; namespace matches folder structure under `DiyMusicCommunity.*`.

## Code style
- **Always use braces `{ }` for `if` / `else` / `for` / `foreach` / `while` bodies**, even single-line.
  ```csharp
  // ✅ correct
  if (id == Guid.Empty)
  {
      throw new ArgumentException("Id cannot be empty.", nameof(id));
  }

  // ❌ forbidden
  if (id == Guid.Empty)
      throw new ArgumentException("Id cannot be empty.", nameof(id));
  ```
- **Never use expression-bodied members (`=>`) for methods or constructors.** Use a full block body instead.
  ```csharp
  // ✅ correct
  public void ChangeRole(UserRole newRole)
  {
      Role = newRole;
  }

  // ❌ forbidden
  public void ChangeRole(UserRole newRole) => Role = newRole;
  ```
- LINQ (`Where`, `Select`, `OrderBy`, …) is fine for collection queries; the restriction above applies only to method/constructor bodies.
