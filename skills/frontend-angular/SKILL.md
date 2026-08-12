# Skill: Frontend Angular

## Purpose
Step-by-step playbook for adding an Angular feature in the DIY Music Community project, following Clean Architecture boundaries, smart/dumb component split, and TDD.

## Boundary
- No API calls in dumb/presentational components.
- No template-driven forms.
- No importing `core/` services directly from `shared/` components.

## Workspace path rule
The frontend source always lives at:
```
<repo-root>/frontend/
```
Never create files under `backend/frontend/` or any path with a leading space (` frontend/`).  
All file operations targeting the frontend must use the absolute path:
```
C:\Users\Sergio.campos\source\repos\Camposgrind\diy-music-community\frontend\
```

## Angular patterns in use (non-negotiable)

### Signals & reactivity
- Use `signal<T>()` for all mutable component state — **not** `BehaviorSubject` or plain fields.
- Use `computed()` for derived state.
- Use `effect()` **only** inside a constructor or injection context; never in lifecycle hooks.
- Smart (page) components use `ChangeDetectionStrategy.OnPush`.

### Dependency injection
- Always use `inject()` at field declaration — **no constructor parameter injection**.
```ts
private readonly bandsApi = inject(BandsApiService);
```

### Inputs & outputs
- Use `input<T>()` / `input.required<T>()` for inputs (signal-based).
- Use `output<T>()` for outputs.
- Never use `@Input()` / `@Output()` decorators.

### Pre-filling forms from state
When a form must be pre-populated (e.g. restoring search state on navigation back):
1. Add an `initialFilters = input<T | null>(null)` to the form component.
2. In the constructor, add an `effect()` that calls `form.patchValue()` when the input is non-null.
3. Pass the saved state signal from the page component: `[initialFilters]="savedFilters()"`.

### State services (cross-route)
Use a `@Injectable({ providedIn: 'root' })` signal-based service to preserve state across navigation.  
Pattern used: `SearchStateService` (`features/home/search-state.service.ts`).
```ts
@Injectable({ providedIn: 'root' })
export class SearchStateService {
  private readonly _state = signal<SearchState | null>(null);
  readonly state = this._state.asReadonly();
  save(state: SearchState): void { this._state.set(state); }
  clear(): void { this._state.set(null); }
}
```
- The page component reads `state()` in `ngOnInit` and restores filters + re-executes the query.
- The page component calls `save()` before every API call so the state is always fresh.
- The page component calls `clear()` on reset.

### Routing
- All feature routes use lazy `loadComponent`.
- Route params are read via `ActivatedRoute.snapshot.paramMap.get('id')` in `ngOnInit`.
- Never use `RouterModule`; import `RouterLink` / `RouterOutlet` per-component.

## Workflow

### 1. Confirm or create the spec
Check `docs/specs/NNN-feature-name.md`. If missing, create/update it first.

### 2. Model
Add or update the TypeScript interface in `infrastructure/api/models/` and re-export from `models/index.ts`.

### 3. Service
Add or extend the API service in `infrastructure/api/`.
- Inject `HttpClient` via `inject()`.
- Return `Observable<T>`.
- Write HTTP service tests with `HttpTestingController`.

### 4. Page (smart) component
- Inject services via `inject()`.
- Manage state with `signal` / `computed`.
- Use `ChangeDetectionStrategy.OnPush`.
- Pass data to dumb components via `input()`.
- React to dumb component events via `output()`.
- Write component tests for state transitions.

### 5. Presentational (dumb) components
- Accept data via `input()` / `input.required()` only.
- Emit events via `output()` only.
- No service injection.
- Selector: `dmc-<component-name>`.

### 6. Routing
Register the page component in `app.routes.ts` using lazy `loadComponent`. Apply guards as required.

### 7. Forms (if applicable)
- Use `ReactiveFormsModule` only.
- Define `FormGroup` in the component class.
- Pre-fill via `initialFilters` input + `effect()` pattern (see above).
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
- [ ] No files created outside `frontend/` root.

## SCSS conventions
All feature SCSS follows the BEM methodology with the component block as root class.

### Card style (shared visual language)
Every card-like element — band cards, discography rows, member cards — uses this pattern:
```scss
.my-card {
  display: flex;
  background: #1a1a1a;
  border: 1px solid #2a2a2a;
  border-radius: 10px;
  overflow: hidden;
  box-shadow:
    0 2px 6px rgba(0,0,0,0.5),
    0 8px 24px rgba(0,0,0,0.4);
  transition: transform 0.22s ease, box-shadow 0.22s ease, border-color 0.22s ease;

  &:hover {
    transform: translateY(-4px) scale(1.01);
    border-color: #8b0000;
    box-shadow:
      0 4px 12px rgba(0,0,0,0.6),
      0 16px 40px rgba(0,0,0,0.5),
      0 0 0 1px rgba(139,0,0,0.25),
      0 8px 32px rgba(139,0,0,0.12);
  }

  // Left red accent bar (always first child in markup)
  &__accent {
    width: 4px;
    flex-shrink: 0;
    background: linear-gradient(180deg, #8b0000 0%, #4a0000 100%);
    opacity: 0.85;
    transition: opacity 0.22s ease;
  }

  &:hover &__accent { opacity: 1; }
}
```

### Section title style (shared)
Every section heading (Discography, Current Members, Past Members, etc.) uses:
```scss
&__title {
  font-family: 'Oswald', sans-serif;
  font-size: 1rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 3px;
  color: #cc2222;
  margin: 0 0 1.5rem;
  padding-bottom: 0.625rem;
  border-bottom: 1px solid #2a2a2a;
  text-shadow: 0 0 18px rgba(139,0,0,0.45);
  position: relative;

  &::after {
    content: '';
    position: absolute;
    bottom: -1px; left: 0;
    width: 48px; height: 2px;
    background: linear-gradient(90deg, #8b0000, transparent);
  }
}
```

### Colour palette
| Token | Value | Usage |
|---|---|---|
| Background | `#111111` | Page background |
| Surface | `#1a1a1a` | Cards |
| Surface raised | `#161616` | Image containers |
| Border | `#2a2a2a` | Card borders |
| Accent red | `#8b0000` | Primary accent, borders |
| Accent red bright | `#cc2222` | Section titles |
| Text primary | `#ffffff` | Headings |
| Text secondary | `#aaa` | Meta info |
| Text muted | `#666` | Years, placeholders |
| Status active | `#6fcf6f` | Active / Present badges |
| Status splitup | `#e06060` | Split-Up badge |
| Status onhold | `#d4a843` | On Hold badge |

## Naming cheat sheet
| Thing | Convention |
|---|---|
| File | `band-list.component.ts` |
| Class | `BandListComponent` |
| Selector | `dmc-band-list` |
| Service file | `band.service.ts` |
| Model file | `band.model.ts` |
| Guard file | `auth.guard.ts` |
| State service | `feature-name-state.service.ts` |
| Barrel index | `models/index.ts` — always re-export new models here |
