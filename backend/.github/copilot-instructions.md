# Copilot Instructions

## Project Guidelines
- Never hardcode magic strings (error codes, route segments, message text, enum-like string values) in production code. Extract error codes/messages to a dedicated static class (e.g. BandErrors.Codes) and validator messages to private const fields. Seed data, EF configurations, and migrations are exempt.