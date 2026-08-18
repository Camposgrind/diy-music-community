# Feature: Administrator Band Management

## Functional goal

Allow administrators to create and update band catalog records while keeping all catalog writes
unavailable to anonymous and non-admin users.

## User story

As an administrator, I want to create and update band records so that the public catalog remains
accurate and curated.

## Acceptance criteria (Given/When/Then checkboxes)

- [x] Given I am authenticated as an Admin, when I submit valid general band data from Home, then a band is created and I am redirected to its detail page.
- [x] Given I am not an Admin, when I view Home, then the add-band control is not available.
- [x] Given I am an Admin and submit invalid general band data, then saving is unavailable and field errors explain the missing required data.
- [x] Given I open the initial creation form, when I provide a formation year, then I can type a four-digit year directly; logo and band-photo management remain out of this flow.
- [x] Given I am authenticated as an Admin, when I submit valid general-info changes from a band detail page, then the band is updated and its full detail is reloaded.
- [x] Given I am not an Admin, when I view a band detail page, then the general-info edit control is not available.
- [x] Given I am authenticated as an Admin, when I submit a valid release from the band detail page, then it is created and the band detail is reloaded.
- [x] Given I am authenticated as an Admin, when I edit a release's title, type, or year, then the release is updated, its unchanged metadata and tracks are preserved, and the band detail is reloaded.
- [x] Given I am not an Admin, when I view discography, then release creation and editing controls are not available.
- [x] Given I am an Admin, when I add or edit a current or past member from band detail, then the member is saved and the complete band detail is reloaded.
- [x] Given I am not an Admin, when I view current or past members, then member-management controls are not available.
- [x] Given a band is SplitUp, when I view its detail, then its persisted last known lineup is shown separately from past members.
- [x] Given an Admin adds a member to a SplitUp band's last known lineup, when they submit the form, then an end year is required and the member is persisted as part of that lineup.
- [ ] Given I submit a duplicate band, release, member, or track identity, when the request is processed, then I receive 409 and no data is changed.
- [ ] Given I am unauthenticated, when I call a band create or update endpoint, then I receive 401.
- [ ] Given I am authenticated without the Admin role, when I call a band create or update endpoint, then I receive 403.
- [ ] Given I am a public visitor, when I browse bands, then no authentication is required.

## API contract

The catalog-management endpoints are pending implementation:

- `POST /api/bands` — Admin only.
- `PUT /api/bands/{id}` — Admin only.
- `POST /api/bands/{bandId}/members` — Admin only.
- `PUT /api/bands/{bandId}/members/{memberId}` — Admin only.
- `POST /api/bands/{bandId}/releases` — Admin only. The request includes the release and all of its tracks.
- `PUT /api/bands/{bandId}/releases/{releaseId}` — Admin only. The request replaces the release's complete track list.

`POST` creates a resource and must return `409 Conflict` when its business identity already
exists; it never silently overwrites catalog data. `PUT` identifies the target by its route ID and
returns `404 Not Found` when that target does not exist.

Business identities are case-insensitive and ignore leading/trailing whitespace:

- Band: `Name + Country`.
- Member: `BandId + Name + StartYear`.
- Release: `BandId + Title + ReleaseDate`; when `ReleaseDate` is absent, `Year` is used.
- Track: `ReleaseId + TrackNumber`.

The request and response fields must be added to `docs/technical/openapi.md` before the endpoints
are delivered.

## Domain rules

- A band belongs to one genre and may have releases and band members.
- A release is managed with its tracks as one aggregate. A release update replaces all its tracks;
  omitting a previously stored track removes it.
- A member with an end year is a past member; otherwise its `IsCurrent` value determines whether it
  is current.
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
- Frontend: Home shows the add-band control only for an Admin, opens the creation modal, and navigates to the created band.
- Frontend: the creation modal validates name, country, and genre before emitting a save request.
- Frontend: the initial creation modal accepts a directly entered four-digit formation year and excludes media fields.
- Frontend: the bands API service posts a `BandWriteRequest` and returns `BandDetailModel`.
- Frontend: the detail page shows an Admin-only edit control, preloads the edit form, updates the band, and reloads its detail.
- Frontend: Admins can create or edit a release from discography; the page reloads after either action.
- Frontend: Admins can add or edit current and past members with one reusable modal; the page reloads after saving.
- Frontend: SplitUp bands show their persisted last known lineup and require an end year for its members.

## Out of scope

- Community submissions.
- Band ownership claims and claim-holder editing.
- Proposal and claim review queues or moderation audit records.
- Editing releases or current/past members.
- Advanced band-section editing.
- Logo and band-photo management, which will be available in a later edit flow.
- General-info editing does not expose description, logo, or photo changes; their stored values are preserved.
- Release-track editing and release cover uploads.
- Member photo, image, and other-band management.
