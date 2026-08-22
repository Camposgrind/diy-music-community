# Feature: Login return navigation

## Status

Partially superseded by `014-admin-only-login.md`. Login remains available only from the dedicated
administrator entry route, rather than from public navigation.

## Functional goal

Keep a visitor on the page they were viewing when they sign in.

## User story

As an administrator using the dedicated sign-in route, I want to return to a safe internal URL
when one is supplied.

## Acceptance criteria (Given/When/Then checkboxes)

- [x] Given I open the administrator login route with a valid internal return URL and authenticate successfully, then I return to the same internal URL, including its route parameters and query string.
- [x] Given Login has no valid internal return URL, when I authenticate successfully, then I am sent to Home.

## Security rules

- The return URL must be a local application path. External or protocol-relative URLs are ignored.

## Test scenarios

- Login component: redirects to a valid `returnUrl` after successfully storing the session.
- Login component: falls back to Home for no or unsafe return URL.

## Out of scope

- Restoring unsaved form contents.
