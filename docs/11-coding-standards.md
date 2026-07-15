# Coding Standards & Guidelines

This document outlines the language conventions, naming patterns, and styles for the Enterprise Workforce Management System (EWMS). Adhering to these standards ensures code consistency, maintainability, and clean architecture boundaries.

---

## Backend (C# / .NET 9)

### 1. Naming Conventions

| Language Element | Case Style | Example | Notes |
| :--- | :--- | :--- | :--- |
| **Classes & Structs** | PascalCase | `EmployeeService` | Use descriptive noun phrases. |
| **Interfaces** | PascalCase | `IEmployeeRepository` | Prefix with an uppercase `I`. |
| **Methods** | PascalCase | `GetByIdAsync(...)` | Use verbs or verb-noun phrases. |
| **Properties** | PascalCase | `FirstName` | - |
| **Local Variables** | camelCase | `employeeId` | - |
| **Method Arguments** | camelCase | `request` | - |
| **Private Fields** | `_camelCase` | `_dbContext` | Prefix with a single underscore. |
| **Constants & static readonly**| PascalCase | `MaxPageSize` | Do not use UPPER_CASE. |

### 2. Async Programming
- **Suffix**: All asynchronous methods must end with the `Async` suffix (e.g., `SaveChangesAsync`, `GetEmployeesAsync`).
- **Cancellation Tokens**: Always pass a `CancellationToken` through the call stack where possible.
- **ConfigureAwait**: Avoid using `.ConfigureAwait(false)` in ASP.NET Core applications as there is no synchronization context.

### 3. Layered Architecture Symbol Patterns
- **Data Transfer Objects (DTOs)**:
  - Input Requests: `[Action][Entity]Request.cs` (e.g., `CreateEmployeeRequest.cs`, `UpdateDepartmentRequest.cs`).
  - Output Responses: `[Entity]Response.cs` (e.g., `EmployeeResponse.cs`, `DepartmentDetailsResponse.cs`).
- **Services**:
  - Interface: `I[Entity]Service.cs` (e.g., `IEmployeeService.cs`).
  - Implementation: `[Entity]Service.cs` (e.g., `EmployeeService.cs`).
- **Repositories**:
  - Interface: `I[Entity]Repository.cs` (located in Domain layer).
  - Implementation: `[Entity]Repository.cs` (located in Infrastructure layer).

### 4. Code Formatting Rules
- Avoid redundant parentheses unless they clarify operator precedence.
- Always use curly braces `{ }` for `if`, `for`, `while` statements, even for single lines.
- Prefer file-scoped namespaces (e.g., `namespace Enterprise.Domain.Entities;` without outer braces) to reduce indentation.
- Use primary constructors in .NET 9 where appropriate for dependency injection in classes (e.g., services, controllers, repositories).

---

## Frontend (TypeScript / Angular)

### 1. File Naming Conventions
Follow the official Angular Style Guide. Use kebab-case with dots to represent the feature type:

- **Components**: `[feature-name].component.ts` (e.g., `employee-list.component.ts`, `employee-form.component.ts`)
- **Services**: `[feature-name].service.ts` (e.g., `employee.service.ts`)
- **Models/Interfaces**: `[feature-name].model.ts` (e.g., `employee.model.ts`)
- **Modules**: `[feature-name].module.ts` (if modules are used instead of Standalone components)
- **Guards**: `[name].guard.ts` (e.g., `auth.guard.ts`)
- **Interceptors**: `[name].interceptor.ts` (e.g., `jwt.interceptor.ts`)

### 2. Class & Symbol Naming

| TypeScript Element | Case Style | Example | Notes |
| :--- | :--- | :--- | :--- |
| **Component Class** | PascalCase | `EmployeeListComponent` | End with `Component` suffix. |
| **Service Class** | PascalCase | `EmployeeService` | End with `Service` suffix. |
| **Models/Interfaces** | PascalCase | `Employee` / `CreateEmployeeRequest`| No `I` prefix for TS interfaces. |
| **Properties & Methods**| camelCase | `loadEmployees()` | - |
| **Variables/Constants** | camelCase / UPPER | `pageIndex` / `API_URL` | Use UPPER_CASE only for global config constants. |

### 3. RxJS & State Conventions
- **Observables suffix**: Suffix Observable variables with a `$` (e.g., `employees$`, `currentUser$`).
- **Unsubscription**: Prevent memory leaks by using `takeUntilDestroyed()` (introduced in Angular 16+) or the `async` pipe in templates.
- **Signals**: For local component state and reactive values, prefer Angular Signals where appropriate (e.g., `const searchTerm = signal('');`).

---

## Architecture Decision Logs (ADR)
All key architectural design decisions must be recorded under `docs/08-architecture-decisions/` with the filename `ADR-[###]-[title].md` and registered in the `decision-log.md` file. This includes selections of frameworks, storage systems, and design patterns.
