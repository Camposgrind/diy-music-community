---
applyTo: "frontend/**/*.ts"
---

# Frontend Instructions — DIY Music Community

## Architecture
```
src/app/
├── core/       # App-wide singletons: auth, interceptors, guards, models
├── shared/     # Stateless reusable presentational components / pipes / directives
└── features/   # Self-contained features: pages + local components + feature service
```

## Component rules
- **Smart (page) components:** inject services, call APIs, manage state.
- **Dumb (presentational) components:** `@Input()` / `@Output()` only — no service injection, no API calls.
- Split any component that exceeds ~150 lines or handles more than one responsibility.
- Selector prefix: `dmc-` (e.g. `dmc-band-list`, `dmc-trust-badge`).

## Forms
- Use **Reactive Forms only**. No template-driven forms.

## HTTP & errors
- All API calls go through a dedicated feature service or a base API service in `core/`.
- `error.interceptor.ts` catches HTTP errors and dispatches toasts — do not handle HTTP errors in components.
- `auth.interceptor.ts` attaches the JWT token from `localStorage`.

## Auth state
- `AuthService` exposes current-user via a signal or `BehaviorSubject`.
- Guards: `auth.guard.ts` (authenticated?) and `role.guard.ts` (role check). Administrator catalog-management routes require `Admin`.

## Testing
- Test runner: **Vitest**. Config in `frontend/vitest.config.ts`. Do not use Karma or Jasmine.
- Angular's `TestBed` and `HttpTestingController` work normally with Vitest.
- Test descriptions: `it('should ...')`.
- See `docs/testing/guidelines.md` for full conventions.

## Naming
- Files: `kebab-case.type.ts` (e.g. `band-list.component.ts`, `band.model.ts`).
- Classes: PascalCase.
- One class per file.

## Must NOT do
- Call HTTP/APIs from dumb/presentational components.
- Use template-driven forms.
- Import `core/` services directly from `shared/` components.
- Use Karma or Jasmine — the project uses Vitest.
- Use default change detection in performance-critical lists (prefer `OnPush`).
