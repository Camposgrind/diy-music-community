# Skill: Documentation Maintenance

## Purpose
Keep all project documentation — specs, ADRs, API docs, README, and AI instructions — in sync with the code on every PR.

## Boundary
- Do not invent behavior not present in the code or spec.
- Do not leave a PR that changes behavior without updating the relevant spec.

## Docs map

| Document | Update when… |
|---|---|
| `docs/specs/NNN-*.md` | Feature behavior changes; check acceptance criteria boxes when done. |
| `docs/adr/NNN-*.md` | An architectural or key technical decision is made. |
| `docs/api/` | An endpoint is added, changed, or removed. |
| `docs/technical/` | Architecture, data model, or layer rules change. |
| `docs/functional/` | User-facing behavior changes (plain language). |
| `docs/testing/guidelines.md` | Test conventions, coverage targets, or commands change. |
| `README.md` | Setup steps, run commands, test users, or feature list changes. |
| `AGENTS.md` | A new skill is added or a golden rule changes. |
| `.github/copilot-instructions.md` | Repo-wide AI rules change. |
| `.github/instructions/*.md` | Path-scoped AI rules change. |

## PR checklist
- [ ] Spec acceptance criteria checked off for completed items.
- [ ] `docs/api/` reflects any endpoint contract changes.
- [ ] ADR added if an architectural decision was made (use `docs/adr/NNN-title.md`).
- [ ] `README.md` updated if setup or run steps changed.
- [ ] No doc claims behavior the code does not implement.
- [ ] No invented or aspirational behavior added to specs.

## ADR template
```markdown
# ADR NNN — <title>

## Status
Accepted | Superseded by ADR-NNN

## Context
What situation or problem triggered this decision?

## Decision
What was decided?

## Consequences
What are the trade-offs and implications?
```

## Spec acceptance criteria format
```markdown
## Acceptance criteria
- [x] Given an Admin submits valid band data, when the request is processed, then a Band is created.
- [ ] Given a non-Admin submits band data, when the request is processed, then a 403 is returned.
```
Use `[x]` when the scenario is implemented and tested; `[ ]` when pending.
