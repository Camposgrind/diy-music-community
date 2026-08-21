# Feature: Chronological discography order

## Functional goal

Show every band's discography from its oldest release to its newest release.

## User story

As a visitor, I want to read a band's releases chronologically so that I can
understand the evolution of its catalogue.

## Acceptance criteria

- [x] Given a band has releases from multiple years, when its detail is
  returned, then releases are ordered by year in ascending order.
- [x] Given two releases share a year and have release dates, when the band
  detail is returned, then the earlier release date appears first.
- [x] Given a release has no year, when the band detail is returned, then it
  appears after releases with a known year.

## API contract

`GET /api/bands/{id}` continues to return `releases`; its list ordering is now
defined as chronological ascending.

## Domain and permission rules

None.

## Test scenarios

- Application: an unsorted band aggregate maps to chronological releases.
- Application: releases without a year sort after dated releases.

## Out of scope

- A user-selectable reverse chronology control.
