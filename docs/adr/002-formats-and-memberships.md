# ADR 002 — Release Formats and Band Member Cross-References

## Status
Accepted

## Context
Two fields were originally modelled as plain strings under the MVP "strings over entities" guardrail:

- `Release.FormatsText` — a free-text description of physical/digital formats (e.g. "Vinyl 12\", CD").
- `BandMember.AlsoInBandsText` — a free-text list of other bands the member has played in.

During early development it became clear that both fields need structured data:

1. **Formats** — a release can have multiple simultaneous formats (Vinyl 12" *and* CD *and* Digital).
   A single string cannot be filtered, validated, or displayed consistently.
   The UI needs to show format badges and allow users to filter the catalog by format.

2. **Cross-band member references** — the UI should render each "also in" band as a hyperlink to
   that band's detail page. A free-text string cannot provide the `BandId` FK needed for navigation.

## Decision

### Release formats
Replace `FormatsText` with a `Format` enum + `ReleaseFormat` join entity.

```
Format (enum): Vinyl7 | Vinyl10 | Vinyl12 | VinylLatheCut | VinylOther | CD | CDR | DVD | Cassette | Digital
ReleaseFormat: Id (PK), ReleaseId (FK → Releases), Format (string-stored enum)
```

- `Release` exposes `AddFormat(Format)`, `RemoveFormat(Format)`, `GetFormats()`.
- Uniqueness enforced at DB level: `UNIQUE(ReleaseId, Format)`.
- Cascade delete: removing a release removes its formats.

### Band member cross-band references
Replace `AlsoInBandsText` with a `BandMemberOtherBand` join entity (lightweight — no full
`Member`/`Person` remodel, which is deferred to post-MVP).

```
BandMemberOtherBand: Id (PK), BandMemberId (FK → BandMembers), OtherBandId (FK → Bands)
```

- `BandMember` exposes `AddOtherBand(Guid)`, `RemoveOtherBand(Guid)`, `GetOtherBands()`.
- Uniqueness enforced: `UNIQUE(BandMemberId, OtherBandId)`.
- Cascade delete: removing a member removes their cross-band links.

## Consequences
- **Positive:** formats are filterable and validated at the domain level; no free-text parsing needed.
- **Positive:** cross-band member links resolve to real `BandId` values, enabling UI hyperlinks.
- **Positive:** both are enforced by DB unique indexes — no duplicates possible.
- **Trade-off:** two extra tables + one migration vs. the original string columns.
- **Trade-off:** `SetDetails` signature on `Release` changed (removed `formatsText` parameter) — any
  callers must be updated.
- **Deferred:** a full `Member` entity with proper many-to-many `BandMembership` (one person across
  many bands) is explicitly out of scope for the MVP. `BandMemberOtherBand` is a stepping stone.

## Implementation notes — consistency rules for BandMemberOtherBand

Because the same person exists as separate `BandMember` rows (one per band), the **Application use
cases** must enforce two consistency rules:

### 1. Name/photo propagation
When a moderator or claim-holder updates a member's name (or future profile image), the use case
must find all `BandMember` rows with the same name across the linked bands and propagate the change.

**Use case responsibility (`Application` layer):**
```
UpdateBandMember_Should_PropagateNameChange_ToOtherBandEntries
```
- Find all `BandMemberOtherBand` records where `OtherBandId` points to bands that also have a
  `BandMember` with the same original name.
- Update those sibling rows atomically in the same `IUnitOfWork.SaveChangesAsync()` call.

### 2. Bidirectional link consistency
When a member is linked to another band (e.g. "Dave in Band X" adds OtherBand → Band E), the use
case must also create the reciprocal link (Dave-in-Band-E adds OtherBand → Band X), and the same
for all other bands Dave already links to.

**Use case responsibility (`Application` layer):**
```
AddOtherBand_Should_CreateReciprocalLinks_OnAllRelatedMembers
RemoveOtherBand_Should_RemoveReciprocalLinks_OnAllRelatedMembers
```
- Query all `BandMember` entries that share the same member name and are already cross-linked.
- Add/remove the corresponding `BandMemberOtherBand` entries so links stay symmetrical.
- All changes in a single unit of work to guarantee consistency.

> These rules live in the Application layer (use-case classes), NOT in the Domain entity.
> The Domain entity only protects single-entity invariants (no duplicates, no empty GUIDs).
> Cross-entity orchestration is an application concern.

## Supersedes
The "Strings for Country / Label / Formats — no extra entities" rule in `copilot-instructions.md`
is narrowed to: **Strings for Country / Label only.**
