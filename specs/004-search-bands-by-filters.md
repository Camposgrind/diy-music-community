# Feature: Search Bands by Filters

## Functional goal
Allow any visitor (anonymous or authenticated) to search and filter the public band catalog by name,
country, and genre, receiving a paginated list of results suitable for display in a table.
Results are capped at 100 matching bands (pre-pagination); queries that exceed that cap are rejected
so the user is forced to narrow their filters.

## User story
As a visitor, I want to search bands by name, country, and/or genre with pagination,
so that I can quickly discover underground/DIY bands relevant to what I am looking for
without being overwhelmed by unfiltered results.

## Acceptance criteria
- [ ] Given bands exist, when I GET /api/bands with no filters, then I receive a paginated list of bands (page 1, default pageSize 20).
- [ ] Given `?name=discharge`, when I GET /api/bands, then only bands whose name contains "discharge" (case-insensitive) are returned.
- [ ] Given `?country=UK`, when I GET /api/bands, then only bands from country "UK" are returned.
- [ ] Given `?genreId=<guid>`, when I GET /api/bands, then only bands belonging to that genre are returned.
- [ ] Given multiple filters are combined, when I GET /api/bands, then all filters are applied with AND logic.
- [ ] Given filters produce more than 100 matching bands, when I GET /api/bands, then the API returns 422 with error code `Band.TooManyResults`.
- [ ] Given `?page=2&pageSize=20`, when I GET /api/bands, then the correct page of results is returned.
- [ ] Given `?pageSize=51`, when I GET /api/bands, then the API returns 400 (pageSize max is 50).
- [ ] Given `?page=0`, when I GET /api/bands, then the API returns 400 (page must be >= 1).

## API contract

### Request
```
GET /api/bands?name={string}&country={string}&genreId={guid}&status={BandStatus}&page={int}&pageSize={int}
```

| Parameter | Type       | Required | Default | Constraints              |
|-----------|------------|----------|---------|--------------------------|
| name      | string     | No       | —       | max 200 chars            |
| country   | string     | No       | —       | max 100 chars            |
| genreId   | Guid       | No       | —       | valid GUID when provided |
| status    | BandStatus | No       | —       | valid enum when provided |
| page      | int        | No       | 1       | >= 1                     |
| pageSize  | int        | No       | 20      | 1–50                     |

### Response — 200 OK
```json
{
  "items": [
	{
	  "id": "guid",
	  "name": "Discharge",
	  "country": "UK",
	  "genre": "D-Beat",
	  "status": "Active",
	  "formationYear": 1977
	}
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 42
}
```

### Errors
| HTTP | Error code            | When                                                     |
|------|-----------------------|----------------------------------------------------------|
| 400  | `Band.InvalidFilter`  | Validation failure (pageSize > 50, page < 1, etc.)      |
| 422  | `Band.TooManyResults` | Filtered result set exceeds 100 bands before pagination  |

## Domain rules
- `name`: case-insensitive partial match (contains).
- `country`: case-insensitive exact match.
- `genreId`: exact match on `Band.GenreId`.
- `status`: exact match on `Band.Status`.
- The 100-band cap is checked on the full filtered set **before** pagination is applied.

## Permission rules
- No authentication required. This is a fully public endpoint.

## Validation rules
| Rule                        | Error message                                    |
|-----------------------------|--------------------------------------------------|
| `page` >= 1                 | "Page must be greater than or equal to 1."       |
| `pageSize` between 1 and 50 | "PageSize must be between 1 and 50."             |
| `name` max 200 chars        | "Name filter must not exceed 200 characters."    |
| `country` max 100 chars     | "Country filter must not exceed 100 characters." |

## Test scenarios
- Unit (Application):
  - `GetBands_WithNoFilters_Should_ReturnPagedList`
  - `GetBands_WithNameFilter_Should_ReturnOnlyMatchingBands`
  - `GetBands_WithCountryFilter_Should_ReturnOnlyMatchingBands`
  - `GetBands_WithGenreFilter_Should_ReturnOnlyMatchingBands`
  - `GetBands_WithCombinedFilters_Should_ApplyAllFilters`
  - `GetBands_WhenResultsExceed100_Should_ReturnTooManyResultsFailure`
  - `GetBands_WithValidPagination_Should_ReturnCorrectPage`
- Integration (Api):
  - `GET_Bands_NoFilters_Should_Return200WithPagedResult`
  - `GET_Bands_WithNameFilter_Should_Return200WithFilteredBands`
  - `GET_Bands_ResultsOver100_Should_Return422WithTooManyResultsError`
  - `GET_Bands_InvalidPageSize_Should_Return400`
  - `GET_Bands_InvalidPage_Should_Return400`

## Out of scope
- Full-text / fuzzy search.
- Sorting by popularity, member count, or release count.
- Filtering by formation year range.
- Cursor-based / keyset pagination.
- Caching / ETags.
