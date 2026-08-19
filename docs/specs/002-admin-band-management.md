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
- [x] Given I am authenticated as an Admin, when I choose to delete a current, past, or last-known-lineup member and confirm the action, then the member is deleted and the band detail reloads without it.
- [x] Given I am an Admin, when I initiate member deletion, then I can cancel in a confirmation modal without changing the catalog.
- [x] Given I edit a current member as a past member, when the refreshed detail loads, then the member appears in Past Members; when I change the band to SplitUp, then Current Members becomes Last Known Lineup.
- [x] Given an Admin creates or updates a band with status SplitUp, when no split-up year is provided, then saving is blocked and the API rejects the request.
- [x] Given an Admin creates or updates a SplitUp band with a split-up year, when the request succeeds, then that year is persisted and returned in the band detail.
- [x] Given a visitor views a SplitUp band with formation and split-up years, when the detail is displayed, then it shows `Years active: FormationYear – SplitUpYear`.
- [x] Given an Admin chooses to delete a release and confirms the action, when the deletion succeeds, then the band detail reloads without that release.
- [x] Given an Admin chooses to delete a band from its detail page and confirms the action, when deletion succeeds, then the full band aggregate is deleted and the visitor is redirected to Home.
- [x] Given an Admin initiates release deletion, when they cancel the confirmation modal, then no release is deleted.
- [x] Given I am authenticated as an Admin, when I view a release detail page, then I can open an edit control for its main information and tracks; non-Admin visitors cannot see that control.
- [x] Given an Admin edits a release, when they add, remove, or move tracks and save, then the complete ordered track list is submitted without manually entering track numbers and the release detail reloads.
- [x] Given an Admin creates a release from a band detail, when saving succeeds, then they are taken to the new release detail; editing an existing release keeps them on band detail.
- [x] Given an Admin edits a release from band detail, when they save its main information, then title, type, date, year, label, and formats are updated while tracks and cover remain unchanged.
- [x] Given an Admin views release detail, when they choose Edit details or Edit tracks, then each flow updates only its respective part of the release.
- [x] Given an Admin edits a release date, when opening the date selector or typing `DD/MM/YYYY`, then they can choose a date from the themed calendar or type it with automatic separators and the API receives ISO format.
- [x] Given an Admin enters more than one label, when editing release details, then the field explains that labels are comma-separated; formats are selected through visible modern chips with selected state.
- [x] Given the API returns any release format using its display label, when an Admin opens release editing, then the matching format chip is selected and saving preserves its canonical format value.
- [x] Given an Admin edits a long track list, when the editor opens, then tracks scroll within a compact list while the modal controls remain available; adding a track focuses its new title field.
- [x] Given an Admin reorders or removes a track, when they use that row's controls, then the selected track is the one moved or removed and the remaining visual order is preserved.
- [x] Given an Admin changes a SplitUp band to Active or OnHold, when the update succeeds, then its last known lineup members become past members.
- [x] Given an Admin changes a non-SplitUp band to SplitUp, when the update succeeds, then all past members with the most recent end year become its last known lineup.
- [x] Given an Admin changes a band with current members to SplitUp, when the update succeeds, then those current members become its last known lineup and receive the split-up year as their end year.
- [x] Given a member card has a long name or several actions, when it is displayed in a narrow card, then its name, status tag, edit action, and delete action remain visible by wrapping responsively.
- [x] Given an Admin successfully creates or updates a band, release, member, or track list, when the operation completes, then a success toast confirms the result; if the operation fails, a red error toast shows the API message or a clear fallback.
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
- `DELETE /api/bands/{bandId}/members/{memberId}` — Admin only; returns `204 No Content`.
- `POST /api/bands/{bandId}/releases` — Admin only. The request includes the release and its initial track list.
- `PUT /api/bands/{bandId}/releases/{releaseId}` — Admin only. Updates release information and formats while preserving tracks and cover image.
- `PUT /api/bands/{bandId}/releases/{releaseId}/tracks` — Admin only. Replaces the complete track list in request order; the API assigns consecutive track numbers.

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

Band requests and details include nullable `splitUpYear`. It is required only when `status` is
`SplitUp`; changing to any other status clears it.

## Domain rules

- A band belongs to one genre and may have releases and band members.
- A SplitUp band requires `SplitUpYear`. A band that is not SplitUp stores no split-up year.
- Moving from SplitUp to another status changes last-known-lineup members to past members. Moving to
  SplitUp changes current members to last known lineup with the split-up year as their end year;
  if there are no current members, it promotes the most recent past lineup (all past members
  sharing the greatest end year) instead.
- Release information and tracks have separate update flows. Replacing tracks is atomic; omitted
  tracks are removed and the submitted order becomes the consecutive track-number order.
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
- Frontend: only Admins see the release-detail edit control; its modal preloads main information and tracks, submits tracks in visual order without track numbers, and reloads the detail after saving.
- Application/API: a release-track update replaces tracks atomically and returns the updated release detail.
- Frontend: release creation navigates to its detail, existing release edits stay on band detail, and release detail exposes separate Admin controls for information and tracks.
- Frontend: Admins can add or edit current and past members with one reusable modal; the page reloads after saving.
- Frontend: SplitUp bands show their persisted last known lineup and require an end year for its members.
- Domain/Application: status transitions maintain last-known-lineup and past-member classifications.
- Frontend: catalog creation and update operations display success toasts, while failed mutations display red error toasts with the API error where available.

## Out of scope

- Community submissions.
- Band ownership claims and claim-holder editing.
- Proposal and claim review queues or moderation audit records.
- Advanced band-section editing.
- Logo and band-photo management, which will be available in a later edit flow.
- General-info editing does not expose description, logo, or photo changes; their stored values are preserved.
- Release cover uploads and cover editing.
- Member photo, image, and other-band management.
