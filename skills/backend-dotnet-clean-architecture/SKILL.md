# Skill: Backend .NET Clean Architecture

## Purpose
Step-by-step playbook for adding a backend feature following Clean Architecture, SOLID, TDD, and SDD in the DIY Music Community project.

## Boundary
- No EF Core in Domain or Application.
- No business logic in Api controllers or Infrastructure.
- No MediatR, no AutoMapper.

## Workflow

### 1. Confirm or create the spec
Check `docs/specs/NNN-feature-name.md`. If missing or incomplete, create/update it before writing any code.

### 2. Domain (if new entities/rules are needed)
1. Add entity to `Domain/Entities/`.
2. Add enums to `Domain/Enums/` if needed.
3. Add repository interface to `Domain/Abstractions/`.
4. Add domain exception if a new invariant needs one.
5. **Write domain unit tests first.**

### 3. Application (use case)
1. Create folder `Application/<Feature>/`.
2. Add request DTO + response DTO.
3. Add FluentValidation validator.
4. Add use-case class with `Handle(request)` returning `Result<T>`.
5. **Write application unit tests first** (mock repository interfaces).

### 4. Infrastructure (persistence)
1. Add `IEntityTypeConfiguration<T>` in `Infrastructure/Persistence/Configurations/`.
2. Add repository implementation in `Infrastructure/Repositories/`.
3. Run `dotnet ef migrations add <MigrationName>`.
4. Register in `Infrastructure/DependencyInjection.cs`.

### 5. Api (endpoint)
1. Add/update controller in `Api/Controllers/`.
2. Controller calls use case → maps `Result<T>` to HTTP response.
3. Add `[Authorize]` attribute where required.
4. **Write Api integration tests** (WebApplicationFactory + SQLite).

### 6. Done criteria
- [ ] Spec updated.
- [ ] Domain tests green.
- [ ] Application tests green.
- [ ] Api integration tests green.
- [ ] `dotnet test` passes.
- [ ] No EF/SQL in Domain or Application.
- [ ] No business logic in Api controller.

## Result pattern cheat sheet
```csharp
// Success
return Result<BandDto>.Success(dto);

// Failure
return Result<BandDto>.Failure(Error.NotFound("Band.NotFound", "Band not found."));
return Result<BandDto>.Failure(Error.Conflict("Band.Duplicate", "A band with the same identity already exists."));

// Controller mapping
if (result.IsFailure)
    return result.Error.Code switch
    {
        "Band.NotFound" => NotFound(result.Error),
        "Band.Duplicate" => Conflict(result.Error),
        _ => BadRequest(result.Error)
    };
return Ok(result.Value);
```
