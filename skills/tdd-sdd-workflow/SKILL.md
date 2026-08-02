# Skill: TDD + SDD Workflow

## Purpose
Enforce the Spec-Driven Development → Test-Driven Development loop on every feature, for both backend and frontend.

## Boundary
- Never write implementation code without a spec.
- Never delete or skip tests to pass CI.

## The loop

```
1. SPEC   → Create or update docs/specs/NNN-feature-name.md
2. TESTS  → Write failing tests derived from AC (Given/When/Then)
3. CODE   → Implement the minimal code to make tests pass
4. GREEN  → dotnet test && npm test both pass
5. DOCS   → Update spec + any affected docs in the same PR
```

### Step 1 — Spec
Use the spec template:
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
- One spec per feature, numbered sequentially (`001-public-band-browsing.md`).
- Spec is the **source of truth**. If code and spec disagree, fix the code (or open a discussion before changing the spec).

### Step 2 — Write failing tests first
- Backend: domain unit → application unit → Api integration.
- Frontend: service → component → guard / form (all with Vitest).
- Name tests so failure messages are self-documenting.

### Step 3 — Implement
- Write the minimum code to make the tests pass.
- Do not add behavior not covered by a test.

### Step 4 — Verify
```bash
dotnet test
npm test
```
Both must be green. Do not merge red.

### Step 5 — Update docs
- Check spec checkboxes (`- [x]`).
- Update `docs/api/` if endpoint contract changed.
- Add an ADR in `docs/adr/` if an architectural decision was made.
- Update `README.md` if setup steps changed.

## Guard rails
- If a spec is missing or ambiguous → **stop, create/clarify the spec, then resume**.
- If a test is failing after implementation → fix the code, not the test (unless the spec changed).
- If coverage drops below threshold → add tests, never lower the threshold.