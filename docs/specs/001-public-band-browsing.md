# Feature: Public Band Browsing

## Functional goal
Allow anonymous users to search and browse the public band catalog using filters (name, country, genre) with paginated results.

## User story
As a visitor, I want to search for bands by name, country, and genre so that I can discover underground music.

## Acceptance criteria (Given/When/Then checkboxes)
- [x] Given I am on the home page, When it loads, Then I see a hero section and search form
- [x] Given the search form is displayed, When I see the country dropdown, Then it contains all world countries loaded from a static JSON
- [x] Given the search form is displayed, When I see the genre dropdown, Then it is populated from the GET /api/genres endpoint
- [x] Given I fill filters and click Search, When the API responds successfully, Then band cards are displayed with name, country, genre, status, and formation year
- [x] Given the search returns results, When there are multiple pages, Then pagination controls are displayed
- [x] Given I click a pagination button, When the new page loads, Then results update accordingly
- [x] Given I click Reset, When the form resets, Then all filters are cleared and results disappear
- [x] Given the API returns 422 (TooManyResults), When the error arrives, Then a toast notification is displayed
- [x] Given the API returns any other error, When the error arrives, Then an error message is displayed in the results area
- [x] Given a search is in progress, When waiting for API response, Then a loading indicator is shown

## API contract
- Base URL (dev): `https://localhost:7294` (set in `environment.ts`; CORS allows `http://localhost:4200`)
- `GET /api/genres` → `GenreModel[]` (id, name)
- `GET /api/bands?name=&country=&genreId=&status=&page=&pageSize=` → `PagedResult<BandListItemModel>`
  - `BandListItemModel`: id, name, country, genre, status, formationYear
  - Error 422: `{ code: "Band.TooManyResults", message: "..." }`
  - Error 400: `{ code: "Band.InvalidFilter", message: "..." }`

## Domain rules
- Band name search is case-insensitive contains match (max 200 chars)
- Country is exact match (max 100 chars)
- GenreId is exact GUID match
- Page defaults to 1, PageSize defaults to 20 (max 50)
- Results capped at 100 bands — returns 422 if exceeded

## Permission rules
- All endpoints are anonymous (no auth required)

## Validation rules
- Frontend: no client-side validation beyond max lengths (server validates)
- Form fields are all optional

## Test scenarios (unit / integration)

### Services
- BandsApiService: builds correct query params, only includes non-empty filters
- GenresApiService: calls correct URL, returns typed array
- CountriesService: fetches static JSON, returns string array

### Components
- BandCardComponent: renders band name, country, genre badge, status, formation year
- BandsResultsComponent: shows loading state, error state, empty state, results grid, pagination
- BandsSearchFormComponent: emits search event with form values, emits reset event, displays countries and genres
- HomeComponent: orchestrates search flow, handles success/error/422, manages pagination

## Out of scope
- Band detail page
- Authentication
- Multi-select for country/genre (backend supports single values only)
