# Functional Overview — DIY Music Community

## Purpose

DIY Music Community is a public catalog for underground and DIY music scenes, including Punk,
Crust, Grindcore, Powerviolence, and D-Beat.

## Public experience

Anyone can browse and search the published catalog, filter bands by name, country, genre, and
status, and view the available band, release, and member information.

## Catalog management

The band catalog is curated by administrators. On the Home page, only an authenticated user with
the `Admin` role can open the initial band-creation form and create a general band profile. They
can also edit its general information from the band detail page. These flows deliberately exclude
logo and photo management, which belongs to a later edit flow. The successful creation flow opens
the new band detail page. Registered non-admin users have no band catalog write access.

Administrators can also add a release or edit its title, type, and year from a band's
discography. Editing preserves the release's existing tracks and other stored release metadata;
track and cover management remain separate future flows.

From band detail, administrators can also manage current and past members. A shared form supports
both types, including a change of type when a member joins or leaves the band; member images and
other-band links remain separate future flows.

## Out of scope

The MVP does not support community band proposals, band claims, claim-holder editing, or a
moderation workflow. There are no `BandProposal`, `BandClaim`, or `ModerationAction` records in
the target data model.

## Source documents

- Data model: `docs/technical/erd.md`
- Authorization and catalog behaviour: `docs/specs/002-admin-band-management.md`
- Decision record: `docs/adr/003-admin-only-band-catalog.md`
