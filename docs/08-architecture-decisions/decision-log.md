# Architectural Decision Log

This log provides a quick-reference index of all major architectural design choices made throughout the lifecycle of the Enterprise Workforce Management System (EWMS). Each record details the context, options considered, final decision, and consequences.

## Decisions Index

| ID | Title | Date | Status | Summary |
| :--- | :--- | :--- | :--- | :--- |
| **ADR-001** | [Clean Architecture Pattern](ADR-001-clean-architecture.md) | 2026-07-14 | **Accepted** | Split application into Domain, Application, Infrastructure, and Presentation layers to decouple business logic from external frameworks. |
| **ADR-002** | [Database Choice: SQL Server](ADR-002-database-choice.md) | 2026-07-14 | **Accepted** | Select Microsoft SQL Server as the primary relational store to support transactional integrity, complex reporting, and enterprise tooling. |

---

## Status Legend
- **Proposed**: Under review/discussion.
- **Accepted**: Decision approved and actively implemented.
- **Rejected**: Discussed but not chosen.
- **Superceded**: Previously accepted, but replaced by a newer decision.
