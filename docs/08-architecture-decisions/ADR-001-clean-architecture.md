# ADR-001: Clean Architecture Pattern

## Status
Accepted

## Date
2026-07-14

## Context
We are building the Enterprise Workforce Management System (EWMS), which is designed to scale across multiple modules (Employees, Leave, Attendance, Payroll) and might be split into microservices in later phases of the roadmap. The codebase needs to remain highly testable, maintainable, and decoupled from third-party frameworks, persistence mechanisms, and user interfaces.

## Options Considered
1. **Traditional Three-Tier Architecture (Presentation -> Business Logic -> Data Access)**:
   - *Pros*: Simple to understand, fast to set up.
   - *Cons*: High coupling between business logic and database configurations (EF Core schemas leak into services; business logic becomes dependent on data layers).
2. **Clean Architecture (Onion/Hexagonal Pattern)**:
   - *Pros*: Business rules (Domain & Application) are placed at the core and have zero dependencies on databases, UI frameworks, or third-party packages. Extremely testable, modular, and easy to refactor. Vertical feature organization allows clean division of modules.
   - *Cons*: Higher initial setup overhead (more projects, mapping layers, abstraction interfaces).

## Decision
We choose **Clean Architecture** as the foundational design pattern. The solution will be divided into the following projects:
- `Enterprise.Domain`: Holds core domain models (Entities, Value Objects), custom exceptions, and repository interfaces. Contains zero dependencies on database engines or external services.
- `Enterprise.Application`: Defines business use cases, services, validation rules, DTOs, and interface definitions.
- `Enterprise.Infrastructure`: Implements persistence (EF Core DbContext, migrations), integrations, caching, and infrastructure concerns.
- `Enterprise.API` (Presentation): Exposes HTTP endpoints, configures middlewares, filters, and processes incoming requests.

Within this structure, feature sub-folders will be organized vertically (e.g., `features/employee`, `features/department`) to ensure that related application code stays close together, making future service extraction straightforward.

## Consequences
- **Positive**:
  - Independent of database/ORM choice. The Domain does not know about SQL Server or EF Core.
  - Domain invariants are strongly protected.
  - Modules are clearly separated, enabling high parallel development.
  - Unit tests can run against domain logic and application handlers without database mocks.
- **Negative**:
  - Requires writing mapping code (e.g., entity to DTO mapping).
  - Additional project files and reference management.
  - A small learning curve for developer teams unfamiliar with directional dependencies.
