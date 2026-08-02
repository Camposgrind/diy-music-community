# Feature: Band Claim Workflow

## User story
As a registered user, I want to claim a band to prove my relationship to it.

## Acceptance criteria
- [ ] Given I'm authenticated, when I submit a claim on a band, it is created Pending and the band TrustStatus becomes ClaimPending.
- [ ] Given I already have a Pending claim on that band, a second claim returns 409.
- [ ] Given a moderator approves my claim, the band becomes Claimed and IsClaimed=true, and the profile shows "Claimed" badge.
- [ ] Given a rejection has no reason, it returns 400.

## API contract
POST /api/bands/{bandId}/claims (User)
GET /api/moderation/claims (Mod/Admin)
POST /api/moderation/claims/{id}/approve (Mod/Admin)
POST /api/moderation/claims/{id}/reject (Mod/Admin)

## Domain rules
- One Pending claim per (user, band).
- Approval sets Band.TrustStatus=Claimed, IsClaimed=true, writes ModerationAction.

## Permission rules
- Submit: User. Review: Moderator/Admin.

## Validation rules
- ClaimType required; rejection reason required.

## Test scenarios
- Application: duplicate pending → conflict; approval updates band trust.
- Integration: claim on unknown band → 404.

## Out of scope
- Disputes, multiple approved claimants edit conflicts.