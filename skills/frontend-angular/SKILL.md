# Skill: Frontend Angular

## Purpose
Step-by-step playbook for adding an Angular feature in the DIY Music Community project, following Clean Architecture boundaries, smart/dumb component split, and TDD.

## Boundary
- No API calls in dumb/presentational components.
- No template-driven forms.
- No importing `core/` services directly from `shared/` components.

## Workflow

### 1. Confirm or create the spec
Check `docs/specs/NNN-feature-name.md`. If missing, create/update it first.

### 2. Model
Add or update the TypeScript model in `core/models/` (e.g. `band.model.ts`).

### 3. Service
Add a feature service in `features/<feature>/` (or `core/services/` if app-wide).
- Inject `HttpClient`; return `Observable<T>`.
- Write HTTP service tests with `HttpTestingController`.

### 4. Page (smart) component
- Inject the feature service.
- Manage state (loading, error, data).
- Pass data to dumb components via `@Input()`.
- React to dumb component events via `@Output()`.
- Write component tests for state transitions.

### 5. Presentational (dumb) components
- Accept data via `@Input()` only.
- Emit events via `@Output()` only.
- No service injection.
- Selector: `dmc-<component-name>`.

### 6. Routing
Register the page component in the feature route config. Apply guards (`auth.guard`, `role.guard`) as required by the spec.

### 7. Forms (if applicable)
- Use `ReactiveFormsModule` only.
- Define `FormGroup` in the component class.
- Wire validation errors to the template.
- Write form validation tests.

### 8. Done criteria
- [ ] Spec updated.
- [ ] Component logic tests green.
- [ ] HTTP service tests green.
- [ ] Form validation tests green (if applicable).
- [ ] `npm test` passes.
- [ ] No API calls in dumb components.
- [ ] Selector uses `dmc-` prefix.

## Naming cheat sheet
| Thing | Convention |
|---|---|
| File | `band-list.component.ts` |
| Class | `BandListComponent` |
| Selector | `dmc-band-list` |
| Service file | `band.service.ts` |
| Model file | `band.model.ts` |
| Guard file | `auth.guard.ts` |