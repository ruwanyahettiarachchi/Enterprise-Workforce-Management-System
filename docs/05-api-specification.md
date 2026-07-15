# API Specification

This document details the RESTful API contract for the Enterprise Workforce Management System (EWMS) backend services (Sprint 1). 

- **Base URL**: `/api/v1`
- **Content Type**: `application/json`

---

## 1. Global Error Structure (RFC 7807)

All non-200 level error responses will return a structured JSON response matching the RFC 7807 standard:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Please refer to the errors property for details.",
  "instance": "/api/v1/employees",
  "errors": {
    "Email": [
      "'Email' must be a valid email address."
    ]
  }
}
```

---

## 2. Employee Endpoints

### 2.1 Get Paginated/Filtered Employees
Retrieve a search-optimized list of employees.

- **HTTP Method**: `GET`
- **Path**: `/api/v1/employees`
- **Query Parameters**:
  - `searchTerm`: (string, optional) Search query for names, email, or titles.
  - `departmentId`: (Guid, optional) Filter by department.
  - `status`: (int, optional) Filter by status (0=Active, 1=Probation, 2=Terminated).
  - `pageNumber`: (int, default: 1) Active page.
  - `pageSize`: (int, default: 10, max: 100) Items per page.
- **Success Response (200 OK)**:
  ```json
  {
    "items": [
      {
        "id": "8f39180c-c6cc-4c12-888e-73c3325c345a",
        "firstName": "John",
        "lastName": "Doe",
        "email": "john.doe@company.com",
        "phone": "+1234567890",
        "jobTitle": "Software Engineer",
        "status": 0,
        "joinDate": "2026-01-15",
        "departmentName": "Engineering"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalCount": 48
  }
  ```

---

### 2.2 Get Employee By ID
Retrieve the full details of a specific employee.

- **HTTP Method**: `GET`
- **Path**: `/api/v1/employees/{id}`
- **Success Response (200 OK)**:
  ```json
  {
    "id": "8f39180c-c6cc-4c12-888e-73c3325c345a",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@company.com",
    "phone": "+1234567890",
    "jobTitle": "Software Engineer",
    "status": 0,
    "joinDate": "2026-01-15",
    "departmentId": "2f4fa81d-e59e-4e4b-980b-222a101f30ab"
  }
  ```
- **Error Responses**:
  - `404 Not Found` (if ID does not exist)

---

### 2.3 Create Employee
Register a new employee record.

- **HTTP Method**: `POST`
- **Path**: `/api/v1/employees`
- **Request Body**:
  ```json
  {
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@company.com",
    "phone": "+1987654321",
    "jobTitle": "HR Coordinator",
    "status": 1,
    "joinDate": "2026-07-01",
    "departmentId": "8f4da89c-a11b-4f4c-880c-333f101a40bc"
  }
  ```
- **Success Response (201 Created)**:
  - Header: `Location: /api/v1/employees/3fa85f64-5717-4562-b3fc-2c963f66afa6`
  - Body: same as the created employee representation.

---

### 2.4 Update Employee
Modify details of an existing employee.

- **HTTP Method**: `PUT`
- **Path**: `/api/v1/employees/{id}`
- **Request Body**: same structure as `POST` request body.
- **Success Response (204 No Content)**

---

### 2.5 Delete Employee
Archive or remove an employee record.

- **HTTP Method**: `DELETE`
- **Path**: `/api/v1/employees/{id}`
- **Success Response (204 No Content)**

---

## 3. Department Endpoints

### 3.1 Get All Departments
- **HTTP Method**: `GET`
- **Path**: `/api/v1/departments`
- **Success Response (200 OK)**:
  ```json
  [
    {
      "id": "2f4fa81d-e59e-4e4b-980b-222a101f30ab",
      "name": "Engineering",
      "code": "ENG",
      "managerId": "8f39180c-c6cc-4c12-888e-73c3325c345a",
      "managerName": "John Doe",
      "employeeCount": 24
    }
  ]
  ```

---

### 3.2 Create Department
- **HTTP Method**: `POST`
- **Path**: `/api/v1/departments`
- **Request Body**:
  ```json
  {
    "name": "Marketing",
    "code": "MKT",
    "managerId": null
  }
  ```
- **Success Response (201 Created)**

---

### 3.3 Delete Department
- **HTTP Method**: `DELETE`
- **Path**: `/api/v1/departments/{id}`
- **Success Response (204 No Content)**
- **Error Responses**:
  - `400 Bad Request` (if active employees are still assigned to the department).

---

## 4. Dashboard Endpoints

### 4.1 Get Dashboard Statistics
Retrieve aggregated stats for dashboard counters and graphs.

- **HTTP Method**: `GET`
- **Path**: `/api/v1/dashboard/stats`
- **Success Response (200 OK)**:
  ```json
  {
    "totalActiveEmployees": 142,
    "departmentDistribution": [
      { "departmentName": "Engineering", "count": 64 },
      { "departmentName": "HR", "count": 12 },
      { "departmentName": "Finance", "count": 8 }
    ],
    "upcomingEvents": [
      {
        "employeeId": "8f39180c-c6cc-4c12-888e-73c3325c345a",
        "employeeName": "John Doe",
        "eventType": "Birthday",
        "eventDate": "2026-07-28"
      }
    ]
  }
  ```
