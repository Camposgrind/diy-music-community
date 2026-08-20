# ADR 004 — Independent CI/CD workflows for the monorepo

## Status

Accepted

## Context

The Angular frontend and ASP.NET Core API live in separate folders and deploy to
separate Azure services. A failed test must prevent production deployment, and
deployment credentials must not be committed to the repository.

## Decision

Use two GitHub Actions workflows, scoped by path and triggered by pushes and
pull requests targeting `master`.

- `frontend-ci-cd.yml` tests and builds `frontend/`, then deploys its immutable
  build artifact to Azure Static Web Apps only for a push to `master`.
- `backend-ci-cd.yml` tests and publishes `backend/`, then deploys its immutable
  publish artifact to Azure App Service only for a push to `master`.
- GitHub authenticates to App Service with a federated user-assigned managed
  identity and OpenID Connect. The Static Web App deployment token remains a
  GitHub repository secret.

## Consequences

- Changes to one application do not deploy the other.
- Pull requests validate affected application code without changing production.
- Deployment happens only after the relevant test and production build pass.
- The frontend backend URL is public, non-sensitive configuration in the
  production Angular environment file. No secrets are shipped to the browser.
