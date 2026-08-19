# Feature: Login return navigation

## Functional goal

Keep a visitor on the page they were viewing when they sign in.

## User story

As a visitor viewing a band or release, I want to return to that same page after logging in.

## Acceptance criteria (Given/When/Then checkboxes)

- [x] Given I am on a public page, when I choose Login and authenticate successfully, then I return to the same internal URL, including its route parameters and query string.
- [x] Given Login has no valid internal return URL, when I authenticate successfully, then I am sent to Home.

## Security rules

- The return URL must be a local application path. External or protocol-relative URLs are ignored.

## Test scenarios

- Login component: redirects to a valid `returnUrl` after successfully storing the session.
- Login component: falls back to Home for no or unsafe return URL.

## Out of scope

- Restoring unsaved form contents.
