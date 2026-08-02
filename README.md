# DIY Music Community

Web platform to catalog underground/DIY music scenes (Punk, Crust, Grindcore,
Powerviolence, D-Beat). Public browsing + community proposals + moderation + band claims.

> 🚧 **Setup in progress.** Full run instructions will be added once the backend and
> frontend are scaffolded. See [`PROJECT_BLUEPRINT.md`](./PROJECT_BLUEPRINT.md) for the
> architecture and [`EXECUTION_STEPS.md`](./EXECUTION_STEPS.md) for the build order.

## Status
- [x] Backend scaffolded
- [ ] Frontend scaffolded
- [ ] Public browsing
- [ ] Auth
- [ ] Proposals
- [ ] Moderation
- [ ] Claims

## Maintenance log
| Date | Change |
|------|--------|
| 2025-07-08 | Fixed high-severity transitive vulnerability in `Microsoft.OpenApi` (pinned to 2.7.5) |
| 2025-07-08 | Removed empty root-level `DiyMusicCommunity.slnx` (real solution lives in `backend/`) |

## Tech stack
- Backend: .NET 10 / ASP.NET Core Web API, EF Core, SQLite (dev) / Postgres (prod)
- Frontend: Angular 17+, TypeScript
- Architecture: Clean Architecture, SOLID, TDD, SDD