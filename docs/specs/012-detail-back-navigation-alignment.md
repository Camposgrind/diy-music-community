# Feature: Detail back-navigation alignment

## Functional goal

Keep the back-navigation arrow visually centred within its button on both band
and release detail pages.

## User story

As a visitor, I want the back-navigation icon to be aligned with its label so
that the detail-page controls look deliberate and easy to scan.

## Acceptance criteria

- [x] Given a visitor views a band detail page, when the Back to Results
  button is rendered, then its arrow is vertically centred in the button.
- [x] Given a visitor views a release detail page, when the Back to Band
  button is rendered, then its arrow is vertically centred in the button.
- [x] Given the browser zoom or device scale changes, when either
  back-navigation button is rendered, then its arrow remains centred without
  a pixel-specific positional adjustment.

## API contract

No API contract changes.

## Domain and permission rules

None.

## Test scenarios

- Static frontend layout verification asserts that both back-navigation arrows
  use the same viewBox-based SVG and a fixed relative icon box.

## Out of scope

- Changes to back-navigation labels, routes, or interaction behaviour.
