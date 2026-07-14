# ADR-002: Database Choice: SQL Server

## Status
Accepted

## Date
2026-07-14

## Context
The Enterprise Workforce Management System (EWMS) requires a highly reliable persistence layer with transactional support (ACID compliance) for complex data relationships, including hierarchical department structures, employee audit histories, leave request state changes, and attendance records. The production target is Microsoft Azure, and local development should align closely with production configurations.

## Options Considered
1. **NoSQL Store (e.g., MongoDB)**:
   - *Pros*: Flexible schema, easy scaling.
   - *Cons*: Weak support for complex multi-record transactions and joins. Workforce metrics (like leave accruals, attendance analysis) are relational by nature.
2. **PostgreSQL**:
   - *Pros*: Open source, robust, excellent JSON and relational capabilities, cheap cloud hosting.
   - *Cons*: Less native alignment with full Microsoft Enterprise stack environments (though EF Core support is excellent).
3. **Microsoft SQL Server (LocalDB / Express locally, Azure SQL in production)**:
   - *Pros*: Native integration with .NET and Entity Framework Core. Excellent performance, enterprise-grade tooling (SSMS), security auditing capabilities, and direct path to Azure SQL hosting.
   - *Cons*: Commercial licensing for large enterprise scale (though free developer/express tiers exist), higher resource consumption locally than PostgreSQL or SQLite.

## Decision
We choose **Microsoft SQL Server** as the primary relational database management system. 
- For local development, we will use SQL Server Express or LocalDB initially, followed by running a SQL Server Linux container in Docker during Sprint 2.
- For production, the database will be hosted on Azure SQL Database.

We will write all persistence access queries using Entity Framework Core (EF Core 9) and utilize migrations to track database schema evolutions.

## Consequences
- **Positive**:
  - Out-of-the-box support for EF Core migrations and features.
  - Consistent developer experience between local, staging, and Azure production.
  - Rich enterprise feature support (Temporal tables, row-level security, indexing features).
- **Negative**:
  - Requires developers to have SQL Server running locally (either native or Docker).
  - Production hosting fees on Azure SQL are typically higher than base PostgreSQL instances.
