# Insurance Risk & Premium Calculator API

A small ASP.NET Core Web API that takes applicant details and returns a
computed **risk score** and **suggested premium**, then persists the quote.
Built as a compact, interview-friendly project for an insurance/brokerage
context (Gallagher).

## Why this project

Most take-home projects are CRUD wrappers around a database. This one
includes an actual **decision engine** (`RiskEngine`) — rules-based scoring
that maps to what underwriters do — which gives you something substantive
to explain and extend in an interview.

## Architecture

```
src/
  InsuranceRiskApi.Domain          -> Entities (Applicant, Quote, CoverageType) - no dependencies
  InsuranceRiskApi.Application     -> Business logic (RiskEngine), interfaces, DTOs - depends only on Domain
  InsuranceRiskApi.Infrastructure  -> EF Core (SQLite), QuoteService (persistence) - depends on Application
  InsuranceRiskApi.Api             -> Controllers, Program.cs, Swagger - depends on Application + Infrastructure
tests/
  InsuranceRiskApi.Tests           -> xUnit tests for RiskEngine
```

Standard layered/clean architecture: dependencies point inward, `RiskEngine`
is pure and has zero infrastructure dependencies, which is exactly what
makes it trivially unit-testable.

## Business rules (the part to talk about in the interview)

**Risk score (0–100)**, starts at a base of 20 and adds:
- Age band: under 25 → +20, 25–40 → +5, 41–60 → +0, 60+ → +15
- Location: hardcoded high-risk city list → +15, else +5 (stand-in for a
  real actuarial ZIP/region risk table)
- Prior claims: +10 per claim, capped at +40

**Premium** = base premium for coverage type (Auto $800 / Home $600 /
Health $1200) × (1 + riskScore / 100). A risk score of 100 doubles the
base premium.

These are deliberately simple and swappable — in the interview you can
talk about how you'd replace the hardcoded rules with a weighted model,
external actuarial tables, or a rules engine.

## Running it

Requires the .NET 8 SDK.

```bash
cd src/InsuranceRiskApi.Api
dotnet run
```

Swagger UI opens at the root (`https://localhost:<port>/`). The SQLite
database (`insurance.db`) is created automatically on first run —
no setup needed.

## Running the tests

```bash
cd tests/InsuranceRiskApi.Tests
dotnet test
```

## API

| Method | Route                          | Description                              |
|--------|--------------------------------|-------------------------------------------|
| POST   | `/api/quotes`                  | Submit applicant, get back a quote        |
| GET    | `/api/quotes/{id}`             | Fetch a single quote                      |
| GET    | `/api/quotes?coverageType=Auto`| List quotes, optionally filtered          |

### Example request

```json
POST /api/quotes
{
  "name": "Jane Doe",
  "age": 29,
  "location": "Chicago",
  "priorClaimsCount": 1,
  "coverageType": "Auto"
}
```

## Possible extensions (if you have the extra time)

- JWT auth with broker/client roles
- Move the hardcoded location risk table to a seeded lookup table
- Add a `PATCH /api/quotes/{id}/status` workflow (Draft → Issued → Bound)
- Dockerfile + docker-compose for one-command startup
