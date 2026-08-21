# Feature: Responsive media in detail pages

## Functional goal

Present band photos, band logos, and release covers in consistent responsive
frames without cropping the uploaded image or allowing a media item to dominate
or collapse the detail-page layout.

## User story

As a visitor, I want media on band and release detail pages to remain legible
and balanced across image dimensions and screen sizes.

## Acceptance criteria

- [x] Given a band photo has a portrait, landscape, or square ratio, when it is
  rendered on a band detail page, then its frame adopts the image's native
  ratio, is bounded by responsive maximum dimensions, and does not crop or show
  artificial empty bands.
- [x] Given a band logo has an unusual ratio, when it is rendered, then it is
  contained in a bounded header area and does not exceed the page width.
- [x] Given a release cover has an unusual ratio, when it is rendered on a
  release detail page, then it is fully visible inside a bounded square frame.
- [x] Given a visitor uses a narrow screen, when either detail page is rendered,
  then the media frame fits the available width and remains visually balanced.

## API contract

No API contract changes.

## Domain and permission rules

None.

## Test scenarios

- The frontend layout verification asserts bounded responsive frames and
  `object-fit: contain` for band photos, logos, and release covers.

## Out of scope

- Cropping, editing, or removing whitespace embedded in uploaded image files.
