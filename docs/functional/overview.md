# Functional Overview — DIY Music Community

## Purpose

DIY Music Community is a public catalog for underground and DIY music scenes, including Punk,
Crust, Grindcore, Powerviolence, and D-Beat.

## Public experience

Anyone can browse and search the published catalog, filter bands by name, country, genre, and
status, and view the available band, release, and member information.

## Catalog management

The band catalog is curated by administrators. Only an authenticated user with the `Admin` role
can create or update a band and its associated catalog content. Registered non-admin users have
no band catalog write access.

## Out of scope

The MVP does not support community band proposals, band claims, claim-holder editing, or a
moderation workflow. There are no `BandProposal`, `BandClaim`, or `ModerationAction` records in
the target data model.

## Source documents

- Data model: `docs/technical/erd.md`
- Authorization and catalog behaviour: `docs/specs/002-admin-band-management.md`
- Decision record: `docs/adr/003-admin-only-band-catalog.md`
