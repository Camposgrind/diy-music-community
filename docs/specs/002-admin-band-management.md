# Feature: Administrator Band Management

## Functional goal

Allow administrators to create and update band catalog records while keeping all catalog writes
unavailable to anonymous and non-admin users.

## User story

As an administrator, I want to create and update band records so that the public catalog remains
accurate and curated.

## Acceptance criteria (Given/When/Then checkboxes)

- [ ] Given I am authenticated as an Admin, when I submit valid band data, then a band is created.
- [ ] Given I am authenticated as an Admin, when I submit valid changes to an existing band, then the band is updated.
- [ ] Given I am unauthenticated, when I call a band create or update endpoint, then I receive 401.
- [ ] Given I am authenticated without the Admin role, when I call a band create or update endpoint, then I receive 403.
- [ ] Given I am a public visitor, when I browse bands, then no authentication is required.

## API contract

The intended catalog-management endpoints are pending implementation:

- `POST /api/bands` — Admin only.
- `PUT /api/bands/{id}` — Admin only.

The request and response fields must be defined alongside the implementation and added to
`docs/technical/openapi.md` before the endpoints are delivered.

## Domain rules

- A band belongs to one genre and may have releases and band members.
- `BandProposal`, `BandClaim`, and `ModerationAction` are not part of the domain model.
- A band has no claim or community-trust state; catalog integrity comes from administrator-only
  write access.

## Permission rules

- Public browsing is anonymous.
- Band creation and updates require the `Admin` role.
- There is no proposal, claim, moderator-review, or band-owner permission path.

## Test scenarios (unit / integration)

- Application: an Admin can create a band; an Admin can update a band.
- API: an unauthenticated caller receives 401 for create and update.
- API: a non-Admin caller receives 403 for create and update.
- API: a public caller can still browse the catalog.

## Out of scope

- Community submissions.
- Band ownership claims and claim-holder editing.
- Proposal and claim review queues or moderation audit records.
