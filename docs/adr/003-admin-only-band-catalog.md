# ADR 003 — Admin-Only Band Catalog

## Status

Accepted

## Context

The original design allowed registered users to submit `BandProposal` records and to claim a
band through `BandClaim` records. Moderators reviewed these requests and each decision was
stored as a `ModerationAction`. This created a contribution and moderation workflow that is not
needed for the MVP.

## Decision

The catalog is maintained exclusively by administrators.

- Only the `Admin` role may create or update bands and their catalog content.
- `BandProposal`, `BandClaim`, and `ModerationAction` are removed from the target data model.
- The claim-derived `Band.TrustStatus` and `Band.IsClaimed` fields are removed from the target
  data model.
- Community proposal, claim, moderation, and claim-holder editing endpoints and UI are out of
  scope.

## Consequences

- The database migration that implements this decision must drop the three retired tables and
  remove the two retired `Band` columns, after application code has stopped depending on them.
- Retire their entities, enums, configurations, repositories, use cases, endpoints, UI routes,
  and tests in the same implementation change.
- Administrator create/update endpoints and screens require `Admin` authorization; callers with
  any other role receive `403`, and unauthenticated callers receive `401`.
- Historical proposal and claim specs are retained as explicitly superseded records rather than
  implementation requirements.
