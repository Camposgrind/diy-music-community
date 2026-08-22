# Feature: Registration automatic login

## Status

Superseded by `014-admin-only-login.md` for the MVP. Public registration is no longer routed or
advertised, because only administrators may create an authenticated session.

## Functional goal

Authenticate a user immediately after a successful account registration.

## User story

As a new visitor, I want to be signed in and taken to Home after creating my account.

## Acceptance criteria (Given/When/Then checkboxes)

- [x] Given I submit valid registration data, when the account is created and login succeeds, then a session is stored and I am redirected to Home.
- [x] Given registration or automatic login fails, when the request completes, then no navigation occurs and an error is shown.

## API contract

- `POST /api/auth/register` creates the account.
- The client then uses `POST /api/auth/login` with the submitted credentials to obtain the existing JWT response.

## Out of scope

- Refresh tokens and email confirmation.
