# Insurance Risk & Premium Calculator API

An ASP.NET Core Web API that takes applicant details and returns a computed risk score and suggested premium, then persists the quote. It includes a rules-based decision engine (`RiskEngine`) modelled on how underwriters assess risk, rather than being a plain CRUD wrapper.

## Architecture

```
src/
  InsuranceRiskApi.Domain          -> Entities (Applicant, Quote, CoverageType), no dependencies
  InsuranceRiskApi.Application     -> Business logic (RiskEngine), interfaces, DTOs; depends only on Domain
  InsuranceRiskApi.Infrastructure  -> EF Core (SQLite), QuoteService; depends on Application
  InsuranceRiskApi.Api             -> Controllers, Program.cs, Swagger; depends on Application + Infrastructure
tests/
  InsuranceRiskApi.Tests           -> xUnit tests for RiskEngine
```

Layered / clean architecture: dependencies point inward. `RiskEngine` is pure and has no infrastructure dependencies, which makes it easy to unit test.

## Business rules

**Risk score (0-100)** starts at a base of 20 and adds:

- **Age band:** under 25 → +20, 25-40 → +5, 41-60 → +0, 61+ → +15
- **Location:** hardcoded high-risk city list → +15, otherwise +5 (a stand-in for a real actuarial region table)
- **Prior claims:** +10 per claim, capped at +40

**Premium** = base premium for coverage type (Auto $800 / Home $600 / Health $1200) × (1 + riskScore / 100). A risk score of 100 doubles the base premium.

The rules are deliberately simple and swappable (for example with a weighted model, external actuarial tables, or a rules engine).

## Getting started

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/AyishathRaeesa/insurance-risk-api.git
cd insurance-risk-api
dotnet run --project src/InsuranceRiskApi.Api
```

Swagger UI opens at the root (`https://localhost:<port>/`). The SQLite database (`insurance.db`) is created automatically on first run.

## Running the tests

```bash
dotnet test
```

## API

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/quotes` | Submit applicant, get back a quote |
| GET | `/api/quotes/{id}` | Fetch a single quote |
| GET | `/api/quotes?coverageType=Auto` | List quotes, optionally filtered |

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

## Roadmap

- JWT auth with broker/client roles
- Move the hardcoded location risk table to a seeded lookup table
- `PATCH /api/quotes/{id}/status` workflow (Draft → Issued → Bound)
- Dockerfile + docker-compose for one-command startup
- 
