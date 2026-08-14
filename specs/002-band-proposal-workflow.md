# Retired: Band Proposal Workflow

> **Status:** Superseded by [ADR 003](../docs/adr/003-admin-only-band-catalog.md). Community proposals are no longer part of the product. Retained only as historical context; do not implement this workflow.

## User story
As a registered user, I want to propose a new band so the community can grow the catalog.

## Acceptance criteria
- [ ] Given I'm authenticated, when I POST a valid proposal, it is created with ReviewStatus=Pending.
- [ ] Given I'm anonymous, when I POST a proposal, I get 401.
- [ ] Given a moderator approves a Pending proposal, a published Band (TrustStatus=CommunityCreated) is created.
- [ ] Given a proposal is already Approved, re-approving returns 409.
- [ ] Given a rejection has no reason, it returns 400.

## API contract
POST /api/band-proposals (User)
GET /api/me/band-proposals (User)
GET /api/moderation/band-proposals (Mod/Admin)
POST /api/moderation/band-proposals/{id}/approve (Mod/Admin)
POST /api/moderation/band-proposals/{id}/reject (Mod/Admin)

## Domain rules
- Only Pending proposals can transition.
- Approval creates a Band and a ModerationAction.

## Permission rules
- Create/list-own: User. Review: Moderator/Admin.

## Validation rules
- Name required; rejection reason required.

## Test scenarios
- Application: approve pending → creates band; approve approved → error; reject w/o reason → validation error.
- Integration: user forbidden on moderation endpoints (403).

## Out of scope
- Editing a proposal after submission.
