# Feature: Administrator catalog deletion

## Functional goal

Allow an administrator to remove catalog records while preserving referential integrity and a
contiguous order for the tracks that remain in a release.

## User story

As an administrator, I want to delete inaccurate bands, members, releases, and tracks so that the
catalog remains accurate.

## Acceptance criteria (Given/When/Then checkboxes)

- [x] Given an Admin deletes a band, when the request succeeds, then the band, its members,
  releases, release formats, tracks, and every `BandMemberOtherBand` link that references that
  band are deleted.
- [x] Given an Admin deletes a member of a band, when the request succeeds, then the member and
  all its `BandMemberOtherBand` links are deleted, regardless of whether it is current, past, or
  part of the last known lineup.
- [x] Given an Admin deletes a release, when the request succeeds, then the release, all its
  tracks, and its release formats are deleted.
- [x] Given an Admin deletes a track, when the request succeeds, then the remaining tracks in the
  same release are renumbered consecutively from one, preserving their relative order.
- [x] Given an Admin deletes all tracks of a release and confirms the action, when the request
  succeeds, then every track of that release is removed while the release itself remains.
- [x] Given an Admin deletes a catalog entity from a band or release detail page, when the request
  succeeds or fails, then a success or red error toast respectively confirms the outcome.
- [x] Given an Admin deletes an unknown resource, when the request is processed, then it receives
  `404 Not Found` and no data changes.
- [ ] Given an unauthenticated or non-Admin caller invokes a deletion endpoint, when the request
  is processed, then it receives `401 Unauthorized` or `403 Forbidden`, respectively.

## API contract

All endpoints require the `Admin` role and return `204 No Content` on success:

- `DELETE /api/bands/{id}`
- `DELETE /api/bands/{bandId}/members/{memberId}`
- `DELETE /api/releases/{releaseId}`
- `DELETE /api/releases/{releaseId}/tracks/{trackId}`
- `DELETE /api/releases/{releaseId}/tracks` — deletes the track list; an existing release with no
  tracks still returns `204 No Content`.

Unknown IDs return the existing structured `404` error (`Band.NotFound`, `Member.NotFound`,
`Release.NotFound`, or `Track.NotFound`).

## Domain rules

- `BandMemberOtherBand.OtherBandId` is restrictive, therefore links to a band being deleted must
  be removed explicitly. Links owned by a deleted member are removed by its cascade relationship.
- Release formats and tracks are cascade children of releases.
- Track numbers are unique per release and must remain contiguous after a track deletion.

## Permission rules

- Only users in the `Admin` role can delete catalog resources.

## Validation rules

- Route IDs must be GUIDs; otherwise the route does not match.
- A nested member or track must belong to the band or release in its route.

## Test scenarios (unit / integration)

- Application: deletion returns not found for absent resources and delegates each successful
  deletion to the persistence abstraction.
- Integration: deleting a band removes all dependent data including restrictive other-band links;
  deleting all tracks retains the release.
- Integration: deleting a member removes all other-band links; deleting a release removes tracks
  and formats; deleting a track renumbers its release.
- API: deletion endpoints return 401 without authentication and 403 to a non-Admin.
- Frontend: successful catalog deletions display success toasts and failed deletions display red error toasts.

## Out of scope

- Deleting genres, users, claims, proposals, or moderation records.
- Soft deletion and restoration.
