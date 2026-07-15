# Functional Specification Document

This document defines the functional requirements, user stories, acceptance criteria, and non-functional constraints for the Enterprise Workforce Management System (EWMS) MVP (Sprint 1).

---

## 1. User Stories & Acceptance Criteria

### Module: Employee Management (General Directory)

#### US-1.1: Create Employee Profile
> As an **HR Manager**,  
> I want to **add a new employee record** with core details,  
> So that **they are officially registered in the system directory**.

**Acceptance Criteria:**
1. **Required Fields**: First Name, Last Name, Email, Job Title, Join Date, DepartmentId, and Status must be provided.
2. **Email Uniqueness**: The email address must be unique across all active and inactive employees.
3. **Valid Formats**: Email must conform to RFC 5322 format.
4. **Given** the HR Manager is on the "Add Employee" form,
   **When** they enter valid details and click "Submit",
   **Then** a new employee record is stored in the database with status `Active`, and a success message is displayed.

---

#### US-1.2: Retrieve & Search Employee Profiles
> As any **Authenticated User**,  
> I want to **search and filter the employee directory**,  
> So that **I can find contact details and roles quickly**.

**Acceptance Criteria:**
1. **Search Parameters**: Users can search by First Name, Last Name, Email, or Job Title.
2. **Filters**: Users can filter results by Department and Status (Active, Terminated, Probation).
3. **Pagination**: Results must support pagination (default page size: 10, configurable to 25, 50, 100).
4. **Given** the directory has 50 records,
   **When** the user searches for "John" and filters by "Engineering",
   **Then** only matching engineering employees with "John" in their name are returned.

---

#### US-1.3: Update Employee Profile
> As an **HR Manager**,  
> I want to **edit an existing employee's details**,  
> So that **their records remain accurate (e.g., changes in address, status, or job title)**.

**Acceptance Criteria:**
1. **Write Boundaries**: Only HR Managers (users with write permissions) can update details. General employees cannot edit others' profiles.
2. **Protected Fields**: The database primary key (`Id`) must be immutable.
3. **Given** the HR Manager edits an employee's job title to "Senior Developer",
   **When** they click "Save Changes",
   **Then** the record is updated in the database, and the change is visible instantly.

---

### Module: Department Management

#### US-2.1: Manage Departments
> As an **HR Manager**,  
> I want to **create and update departments**,  
> So that **I can structure the company organization chart**.

**Acceptance Criteria:**
1. **Name Uniqueness**: Department names must be unique.
2. **Department Head Assignment**: A department can optionally have an `EmployeeId` assigned as its Manager.
3. **Deletion Safety**: A department cannot be deleted if there are still active employees assigned to it.
4. **Given** a department has 3 active employees,
   **When** the HR Manager attempts to delete the department,
   **Then** the system prevents deletion and returns a business validation error.

---

### Module: Dashboard

#### US-3.1: View Workforce Metrics
> As an **Authenticated User**,  
> I want to **see high-level statistics and upcoming events on the dashboard**,  
> So that **I get a quick summary of the company state**.

**Acceptance Criteria:**
1. **Active Employee Counter**: Displays the total count of active employees.
2. **Department Chart Breakdown**: Exposes employee distributions across departments in a graphical chart.
3. **Upcoming Events Widget**: Lists employee birthdays and work anniversaries occurring within the next 30 days.

---

## 2. Non-Functional Requirements (NFRs)

### Performance & Scalability
- **Response Time**: Read-only directory searches must return in less than 200ms under a load of 100 concurrent requests.
- **Payload Limits**: API response objects must be compressed and paginated to keep JSON sizes below 50KB.

### Usability & Browser Support
- **Cross-Browser Compatibility**: The Angular frontend must render consistently on Google Chrome, Mozilla Firefox, Safari, and Microsoft Edge.
- **Responsiveness**: The UI layout must resize fluidly down to mobile dimensions (minimum width: 320px).

### Security & Data Protection
- **Auditing**: Every employee update must record the creation/update timestamps and the user who modified it.
- **Input Sanitization**: All text input must be sanitized before processing to prevent SQL Injection and Cross-Site Scripting (XSS).
