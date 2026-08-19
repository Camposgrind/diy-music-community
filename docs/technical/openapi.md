# OpenAPI Contract — DIY Music Community API

> **Version:** v1  
> **Base URL (local dev):** `http://localhost:5071/api`  
> **Swagger UI (local dev):** `http://localhost:5071/swagger`  
> **Content-Type:** `application/json`  
> **Authentication:** None required for public endpoints.

---

## Table of contents

1. [General conventions](#general-conventions)
2. [Shared schemas](#shared-schemas)
   - [Error](#error)
   - [PagedResult\<T\>](#pagedresultt)
   - [BandListItemModel](#BandListItemModel)
3. [Endpoints](#endpoints)
   - [GET /api/bands](#get-apibands)
   - [Admin catalog management](#admin-catalog-management)
4. [Enum reference](#enum-reference)
5. [Error code catalogue](#error-code-catalogue)
6. [Frontend integration guide](#frontend-integration-guide)

---

## General conventions

| Convention | Detail |
|---|---|
| All dates | ISO 8601 UTC (`2026-08-05T00:00:00Z`) |
| Enums | Returned as **strings** (`"Active"`, not `0`) |
| Pagination | Always `page` (1-based) + `pageSize` + `totalCount` |
| Errors | Always `{ code, message }` — never raw string bodies |
| Blocked bands | **Never** included in any public response |
| 422 cap rule | Queries matching > 100 bands are rejected; caller must narrow filters |

---

## Shared schemas

### Error

Returned for `400` and `422` responses.

```json
{
  "code": "Band.TooManyResults",
  "message": "Your search returned more than 100 bands. Please refine your filters to narrow the results."
}
```

| Property | Type   | Description |
|----------|--------|-------------|
| `code`   | string | Machine-readable error identifier (see [Error code catalogue](#error-code-catalogue)) |
| `message`| string | Human-readable explanation suitable for display |

---

### PagedResult\<T\>

Generic pagination wrapper. Returned by all list endpoints.

```json
{
  "items": [ /* array of T */ ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 42
}
```

| Property     | Type    | Description |
|--------------|---------|-------------|
| `items`      | T[]     | Items on the current page |
| `page`       | integer | Current page number (1-based) |
| `pageSize`   | integer | Maximum items per page |
| `totalCount` | integer | Total items matching filters (pre-pagination) |

---

### BandListItemModel

Single row in the band list table.

```json
{
  "id": "ba4dc0de-beef-cafe-f00d-b00000000001",
  "name": "Convulsions",
  "country": "Spain",
  "genre": "Grindcore",
  "status": "Active",
  "formationYear": 2016
}
```

| Property        | Type    | Nullable | Description |
|-----------------|---------|----------|-------------|
| `id`            | UUID    | No       | Unique band identifier |
| `name`          | string  | No       | Band name |
| `country`       | string  | No       | Country of origin (e.g. `"UK"`, `"Spain"`) |
| `genre`         | string  | No       | Genre name resolved from the genre catalog |
| `status`        | string  | No       | Activity status — see [BandStatus enum](#bandstatus) |
| `formationYear` | integer | **Yes**  | Year the band was formed; `null` if unknown |

---

## Endpoints

### GET /api/bands

Search and filter the public band catalog with optional pagination.

**Availability:** Public — no authentication required.

#### Request

```
GET /api/bands?name=&country=&genreId=&status=&page=&pageSize=
```

##### Query parameters

| Parameter  | Type         | Required | Default | Constraints                          | Description |
|------------|--------------|----------|---------|--------------------------------------|-------------|
| `name`     | string       | No       | —       | max 200 chars                        | Partial, case-insensitive contains match on band name |
| `country`  | string       | No       | —       | max 100 chars                        | Exact, case-insensitive match on country of origin |
| `genreId`  | UUID         | No       | —       | valid UUID                           | Exact match on the band's genre identifier |
| `status`   | BandStatus   | No       | —       | `Active` \| `SplitUp` \| `OnHold`   | Exact match on activity status |
| `page`     | integer      | No       | `1`     | ≥ 1                                  | Page number (1-based) |
| `pageSize` | integer      | No       | `20`    | 1 – 50                               | Items per page |

> **Note:** All filters combine with **AND** logic. Omitting a filter means "no restriction" for that field.

##### Example requests

```
# All bands, first page
GET /api/bands

# D-Beat bands from the UK, page 2
GET /api/bands?country=UK&genreId=c3d4e5f6-a7b8-9012-cdef-123456789012&page=2&pageSize=10

# Active bands whose name contains "discharge"
GET /api/bands?name=discharge&status=Active
```

---

### Admin catalog management

All endpoints in this section require a bearer token for a user with the `Admin` role. Anonymous
requests receive `401`; authenticated non-admin requests receive `403`.

| Method and path | Purpose | Success |
|---|---|---|
| `POST /api/bands` | Create a band | `201` with `BandDetailModel` |
| `PUT /api/bands/{id}` | Update the band profile | `200` with `BandDetailModel` |
| `POST /api/bands/{bandId}/members` | Add a current or past member | `201` with `BandMemberModel` |
| `PUT /api/bands/{bandId}/members/{memberId}` | Update a member | `200` with `BandMemberModel` |
| `POST /api/bands/{bandId}/releases` | Create a release with its initial track list | `201` with `ReleaseDetailModel` |
| `PUT /api/bands/{bandId}/releases/{releaseId}` | Update release information and formats, preserving tracks and cover | `200` with `ReleaseDetailModel` |
| `PUT /api/bands/{bandId}/releases/{releaseId}/tracks` | Replace the complete ordered track list | `200` with `ReleaseDetailModel` |
| `DELETE /api/bands/{id}` | Delete a band and its catalog dependents | `204` |
| `DELETE /api/bands/{bandId}/members/{memberId}` | Delete a member and its other-band links | `204` |
| `DELETE /api/releases/{releaseId}` | Delete a release, its tracks, and formats | `204` |
| `DELETE /api/releases/{releaseId}/tracks/{trackId}` | Delete a track and renumber later tracks | `204` |

Band request fields are `name`, `country`, `genreId`, `status`, `location`, `formationYear`,
`splitUpYear`, `description`, `logoImageUrl`, `bandImageUrl`, `musicUrlPortal`, and `bandContact`. `name`,
`country`, and `genreId` are required.

`splitUpYear` is required when `status` is `SplitUp`; it is cleared when the band changes to any
other status. A SplitUp band detail includes both years so clients can display its active period.

Member request fields are `name`, `instrument`, `startYear`, `endYear`, `isCurrent`, and
`isLastKnownLineup`. An end year makes the member past regardless of `isCurrent`. A last-known
lineup member must have an end year and is used to identify a split-up band's final lineup.

Release request fields are `title`, `releaseType`, `releaseDate`, `year`, `labelText`,
`coverImageUrl`, `formats`, and `tracks`. Each track has only `title`; the API derives its
consecutive number from the request order. The release-information `PUT` preserves both tracks and
cover image. The `/tracks` `PUT` accepts `{ "tracks": [{ "title": "..." }] }` and replaces tracks
atomically, so omitted tracks are removed.

`POST` never upserts. It returns `409 Conflict` with `Catalog.Duplicate` for a duplicate business
identity. `PUT` returns `404` for an unknown resource and `409` when the change collides with a
different existing resource. Invalid request data returns `400` with `Catalog.InvalidRequest`.
Write responses contain the current persisted DTO, so the frontend can update local state without a
follow-up detail request.

Deletion endpoints return no response body. An unknown resource returns `404` with the matching
resource error code. Deleting a band also removes its releases, release formats, tracks, members,
and all `BandMemberOtherBand` links that reference the deleted band. All deletion endpoints are
Admin-only.

---

#### Responses

##### 200 OK

Returns a `PagedResult<BandListItemModel>`.

```json
{
  "items": [
	{
	  "id": "ba4dc0de-beef-cafe-f00d-b00000000001",
	  "name": "Convulsions",
	  "country": "Spain",
	  "genre": "Grindcore",
	  "status": "Active",
	  "formationYear": 2016
	}
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 1
}
```

##### 400 Bad Request

Returned when query parameters fail validation.

```json
{
  "code": "Band.InvalidFilter",
  "message": "PageSize must be between 1 and 50."
}
```

Common triggers:

| Trigger | Message |
|---|---|
| `page < 1` | `"Page must be greater than or equal to 1."` |
| `pageSize > 50` | `"PageSize must be between 1 and 50."` |
| `name` > 200 chars | `"Name filter must not exceed 200 characters."` |
| `country` > 100 chars | `"Country filter must not exceed 100 characters."` |

##### 422 Unprocessable Entity

Returned when the filtered result set exceeds **100 bands** before pagination. The caller must narrow the filters (add `name`, `country`, `genreId`, or `status`).

```json
{
  "code": "Band.TooManyResults",
  "message": "Your search returned more than 100 bands. Please refine your filters to narrow the results."
}
```

---

## Enum reference

### BandStatus

| Value      | Description |
|------------|-------------|
| `Active`   | Band is currently active |
| `SplitUp`  | Band has broken up |
| `OnHold`   | Band is on hiatus |

> Enums are serialised as **strings** in all requests and responses.

---

## Error code catalogue

| Code | HTTP Status | Description |
|---|---|---|
| `Band.InvalidFilter` | 400 | One or more query parameters failed validation |
| `Band.TooManyResults` | 422 | Filters matched > 100 bands — refine the search |
| `Catalog.Duplicate` | 409 | A catalog business identity already exists |
| `Catalog.InvalidRequest` | 400 | Catalog input failed validation |

---

## Frontend integration guide

### Angular service example

```typescript
// band.model.ts
export interface BandListItem {
  id: string;
  name: string;
  country: string;
  genre: string;
  status: 'Active' | 'SplitUp' | 'OnHold';
  formationYear: number | null;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface ApiError {
  code: string;
  message: string;
}

export interface GetBandsParams {
  name?: string;
  country?: string;
  genreId?: string;
  status?: 'Active' | 'SplitUp' | 'OnHold';
  page?: number;       // default 1
  pageSize?: number;   // default 20, max 50
}
```

```typescript
// band.service.ts
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BandListItem, GetBandsParams, PagedResult } from './band.model';

@Injectable({ providedIn: 'root' })
export class BandService {
  private readonly baseUrl = '/api/bands';

  constructor(private http: HttpClient) {}

  getBands(params: GetBandsParams = {}): Observable<PagedResult<BandListItem>> {
	let httpParams = new HttpParams();

	if (params.name)     httpParams = httpParams.set('name', params.name);
	if (params.country)  httpParams = httpParams.set('country', params.country);
	if (params.genreId)  httpParams = httpParams.set('genreId', params.genreId);
	if (params.status)   httpParams = httpParams.set('status', params.status);
	if (params.page)     httpParams = httpParams.set('page', params.page);
	if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);

	return this.http.get<PagedResult<BandListItem>>(this.baseUrl, { params: httpParams });
  }
}
```

### Handling errors

```typescript
import { HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiError } from './band.model';

this.bandService.getBands(filters).pipe(
  catchError((err: HttpErrorResponse) => {
	const apiError = err.error as ApiError;

	if (err.status === 400) {
	  // Validation error — show apiError.message to the user
	}
	if (err.status === 422 && apiError.code === 'Band.TooManyResults') {
	  // Prompt user to add more filters
	}

	return throwError(() => apiError);
  })
);
```

### Pagination in the table component

```typescript
// band-list.component.ts
currentPage = 1;
pageSize = 20;
totalCount = 0;

loadBands(): void {
  this.bandService.getBands({
	...this.activeFilters,
	page: this.currentPage,
	pageSize: this.pageSize,
  }).subscribe(result => {
	this.bands = result.items;
	this.totalCount = result.totalCount;
  });
}

onPageChange(page: number): void {
  this.currentPage = page;
  this.loadBands();
}
```

### Genre IDs (seeded catalog)

Use these UUIDs as `genreId` filter values in the UI (e.g. populate a `<select>` dropdown by calling a future `GET /api/genres` endpoint — for now they are static):

| Genre | UUID |
|---|---|
| Grindcore | `a1b2c3d4-e5f6-7890-abcd-ef1234567890` |
| Crust | `b2c3d4e5-f6a7-8901-bcde-f12345678901` |
| D-Beat | `c3d4e5f6-a7b8-9012-cdef-123456789012` |
| Powerviolence | `d4e5f6a7-b8c9-0123-def0-234567890123` |
| Punk | `e5f6a7b8-c9d0-1234-ef01-345678901234` |
| Noise | `f6a7b8c9-d0e1-2345-f012-456789012345` |
| Goregrind | `a7b8c9d0-e1f2-3456-0123-567890123456` |
| Gorenoise | `b8c9d0e1-f2a3-4567-1234-678901234567` |
| Death Metal | `c9d0e1f2-a3b4-5678-2345-789012345678` |
| Death-Grind | `d0e1f2a3-b4c5-6789-3456-890123456789` |

---

*Last updated: 2026-08-06 — spec `004-search-bands-by-filters.md`*
