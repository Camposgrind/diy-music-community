# Feature: Global HTTP request loader

## Functional goal

Show a centered loading animation while the application waits for one or more HTTP requests, without requiring each screen to implement its own pending state.

## User story

As a visitor or administrator, I want a consistent visual indication while searches, sign-ins, and detail pages are loading so I know that the application is processing my action.

## Acceptance criteria

- [x] Given one or more pending requests made with Angular `HttpClient`, when any remains in progress, then a centered full-screen loader is visible.
- [x] Given multiple concurrent requests, when one finishes, then the loader remains visible until the final request completes, fails, or is cancelled.
- [x] Given a request that completes or fails, when its observable finalizes, then its pending state is removed.
- [x] Given any application route, when the loader is rendered, then the optimized GIF is requested from the absolute public asset URL.
- [x] Given the source animation includes an opaque neutral canvas, when the loader asset is generated, then its 75 frames are cropped around the symbol and its neutral canvas is converted to real transparency.
- [x] Given the optimized GIF has transparent pixels, when it is displayed over the overlay, then its symbol remains clearly visible without a background panel.

## Technical approach

- `LoadingService` owns a signal-based counter of pending requests.
- A functional HTTP interceptor increments the counter before forwarding a request and decrements it from `finalize`.
- `GlobalLoaderComponent` is mounted once in the app shell and reacts to the service signal.

## Test scenarios

- The service never returns a negative pending count and supports concurrent operations.
- The interceptor activates the loader for pending requests and always clears it after both success and error.
