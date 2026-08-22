# Feature: Release cover administration

## Functional goal

Allow administrators to upload and replace a release cover through the existing temporary-file flow, storing a stable Azure Blob Storage path and returning a read-only SAS URL.

## User story

As an administrator, I want to add or replace a release cover without storing SAS URLs or overwriting the image when I edit its textual data.

## Acceptance criteria

- [x] Given an administrator and an existing release, when they upload a valid PNG, JPG, or JPEG cover, then the API stores a temporary validated file scoped to that release.
- [x] Given a valid temporary release cover, when it is confirmed, then it is uploaded to `releases/{releaseId}/cover/{fileId}.{extension}`, its stable path is persisted, and its temporary local file is deleted.
- [x] Given a release cover replacement, when the new path persists successfully, then deletion of the prior blob is attempted afterwards without invalidating the response on deletion failure.
- [x] Given a release detail response, when a valid cover blob path exists, then `coverImageUrl` contains a read-only, temporary SAS URL; when it does not exist, it is null.
- [x] Given textual release creation or update, when it contains a legacy cover field, then it does not overwrite the persisted blob path.
- [x] Given a non-admin, invalid file, wrong release, mismatched type, or expired temporary file, when an image endpoint is called, then the API responds using the established authorization and error conventions.

## API contract

### `POST /api/releases/{releaseId}/images/temporary`

Admin only; `multipart/form-data` fields: `file` and `imageType` (`ReleaseCover`). Returns temporary-file metadata.

### `POST /api/releases/{releaseId}/images/confirm`

Admin only. Receives `{ "imageType": "ReleaseCover", "temporaryFileId": "..." }` and returns the release ID, temporary SAS image URL, and internally persisted blob path.

## Domain rules

- `ReleaseCoverBlobPath` is the database source of truth; SAS URLs are never persisted.
- Only PNG and JPEG content validated through existing magic-byte validation is accepted.
- Existing temporary storage, validation, Blob Storage, and URL resolver services are generalized rather than duplicated.

## Permission rules

- The existing `[Authorize(Roles = "Admin")]` release-management policy applies.

## Test scenarios

- Domain: a release accepts a nonempty cover blob path.
- Application: confirmation persists the path and deletes the temporary file only after successful persistence.
- Application: invalid ownership, type, or expiry does not upload.
- Integration: Swagger documents both multipart and confirmation endpoints; non-admin access is forbidden.

## Out of scope

- Angular UI for release covers.
- Image management for other resource types.
