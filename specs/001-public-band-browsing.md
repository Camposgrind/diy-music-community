# Feature: Public Band Browsing

## Functional goal
Let anyone browse and filter the band catalog without authentication.

## User story
As an anonymous visitor, I want to search and filter bands so I can discover underground music.

## Acceptance criteria
- [ ] Given bands exist, when I GET /api/bands, then I receive a paginated list.
- [ ] Given I pass ?genreId=X, then only bands of that genre are returned.
- [ ] Given a band has TrustStatus=Blocked, then it is excluded from results.
- [ ] Given an unknown band id, when I GET /api/bands/{id}, then I get 404.

## API contract
GET /api/bands?name&genreId&country&status&page&pageSize → PagedResult<BandListItemDto>
GET /api/bands/{id} → BandDetailDto | 404

## Domain rules
- Blocked bands are never publicly listed.

## Permission rules
- No authentication required.

## Validation rules
- pageSize max 50.

## Test scenarios
- Unit: filter logic excludes Blocked.
- Integration: GET /api/bands returns 200 + pagination; unknown id → 404.

## Out of scope
- Full-text search, sorting by popularity.