# Feature: Admin-only login

## Functional goal

Keep the MVP catalogue public while allowing only administrators to create an authenticated
session.

## User story

As a public visitor, I want the catalogue navigation to remain focused on browsing. As an
administrator, I want a dedicated sign-in route so that I can manage the catalogue after I
authenticate.

## Acceptance criteria (Given/When/Then checkboxes)

- [x] Given I am an anonymous visitor, when I browse any public route, then the header only
  exposes Home and does not expose Login or registration.
- [x] Given I know the administrator sign-in route, when I open it, then I can use the existing
  login form.
- [x] Given I am already authenticated as an administrator, when I browse the catalogue, then my
  username and Log Out control remain available in the header.
- [x] Given valid credentials belong to a user without the Admin role, when they call the login
  endpoint, then no JWT is issued and the endpoint returns the existing invalid-credentials error.
- [x] Given valid credentials belong to an administrator, when they call the login endpoint, then
  a JWT is issued as before.

## API contract

- `POST /api/auth/login` continues to accept email-or-username and password.
- A successful response (`200`) is available only to users in the `Admin` role.
- Invalid credentials and authenticated users without the `Admin` role both return the existing
  `400` `Auth.InvalidCredentials` response, so the API does not disclose role membership.

## Permission rules

- Authentication sessions are restricted to the `Admin` role for this MVP.
- The public header does not advertise either sign-in or self-registration.

## Test scenarios (unit / integration)

- Header component: anonymous navigation exposes Home but no Login link; an authenticated
  administrator retains Log Out.
- Routes: the login form is mounted at the administrator-only entry route and the former public
  login and registration routes are unavailable.
- Login use case: a non-Admin successful identity lookup returns invalid credentials and does not
  generate a JWT; an Admin receives a token.

## Out of scope

- Hiding a client-side route is not an authorization boundary.
- Changing how administrators are provisioned or adding invitation flows.
