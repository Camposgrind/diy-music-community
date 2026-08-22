# Feature: Band image administration

## Functional goal

Allow an administrator to upload temporarily and confirm a main photo or logo for an existing band, persist a stable file identifier, and return a temporary read URL.

## User story

As an administrator, I want to replace a band's photo or logo so I can keep its profile up to date without storing temporary URLs or storage credentials in the database.

## Acceptance criteria

- [x] Given an administrator and an existing band, when they upload a valid PNG, JPG, or JPEG as `bandPhoto` or `bandLogo`, then the API stores a validated temporary file and returns its opaque identifier and detected metadata.
- [x] Given a temporary file associated with the same band and image type, when an administrator confirms it before expiry, then the file is stored at a stable path and that path is persisted on the band.
- [x] Given an image replacement, when the new file is persisted successfully, then deletion of the former definitive file is attempted only afterwards and a deletion failure does not invalidate the successful update.
- [x] Given an invalid, empty, oversized, expired, or mismatched temporary file, when an administrator uploads or confirms it, then the API returns the standard structured error response and does not alter the band.
- [x] Given a non-administrator or an unknown band, when they call either image endpoint, then the API rejects the request using the existing authorization and not-found conventions.
- [x] Given a persisted image path, when a band is returned, then any read URL is generated on demand and the database never stores a SAS URL.
- [x] Given expired or confirmed temporary files, when cleanup runs, then only temporary local files are deleted.
- [x] Given a band detail with a persisted image blob path, when its blob no longer exists or storage cannot resolve it, then the public image URL is null and the incident is logged without failing the band-detail response.
- [x] Given a textual band update containing legacy image fields, when it is saved, then existing image blob paths remain unchanged.

## API contract

### `POST /api/bands/{bandId}/images/temporary`

Authenticated administrators only. Receives `multipart/form-data` with `file` and `imageType` (`bandPhoto` or `bandLogo`).

Returns `200 OK`:

```json
{
  "temporaryFileId": "opaque-id",
  "originalFileName": "image.png",
  "sanitizedFileName": "image.png",
  "detectedContentType": "image/png",
  "extension": "png",
  "size": 123456,
  "previewUrl": null
}
```

### `POST /api/bands/{bandId}/images/confirm`

Authenticated administrators only. Receives:

```json
{
  "imageType": "bandPhoto",
  "temporaryFileId": "opaque-id"
}
```

Returns `200 OK` with the band identifier, image type, stable blob path, and a read-only URL generated for the configured lifetime.

## Domain rules

- A band has independent paths for its main photo and logo.
- Supported image content is PNG, JPEG, and JPG only; detected magic bytes determine the saved extension and content type.
- Definitive paths follow `bands/{bandId}/{photo|logo}/{fileId}.{detectedExtension}`.
- A temporary file belongs to exactly one band and one image type and cannot be confirmed after expiration.

## Permission rules

- Both endpoints require the existing `Admin` role authorization; no parallel authorization mechanism is introduced.

## Validation rules

- Reject missing and empty files.
- Enforce the configured maximum image size.
- Treat the supplied extension and MIME type only as supporting information; require valid PNG or JPEG magic bytes.
- Generate unguessable temporary identifiers and never expose physical local paths.

## Architecture decision

The production storage abstraction is `IBlobStorageService`, implemented with Azure Blob Storage to generate read-only SAS URLs for a configured lifetime. The database persists stable `BandPhotoBlobPath` and `BandLogoBlobPath`, not SAS URLs.

Azure Blob Storage is the approved definitive-media storage for this project. Short-lived local files are retained only until successful confirmation and are deleted immediately after the database update succeeds. Expired temporary files are also removed opportunistically on every upload or confirmation.

## Compatibility decision

The existing public fields `bandImageUrl` and `logoImageUrl` remain the response contract and now contain short-lived read-only SAS URLs. Blob paths are never exposed. No null-path fallback search is implemented: the feature did not precede the definitive `bands/{bandId}/photo|logo/{fileId}` convention, and choosing an arbitrary blob would be unsafe when duplicate candidates exist.

## Test scenarios

- Unit: image validator accepts PNG and JPEG signatures and rejects malformed, unsupported, empty, and oversized content.
- Unit: temporary-file service enforces ownership, image type, and expiry.
- Application: confirmation persists only a stable path and attempts prior-file deletion after persistence.
- Integration: administrator upload and confirmation succeed; non-admin, invalid file, wrong band or type, and expired temporary file receive the appropriate responses.

## Out of scope

- [x] Angular UI for administrators: local preview, drag-and-drop, temporary upload, confirmation, and in-place SAS URL update.
- Release cover images.
- Public writes and direct client-side Azure uploads.
