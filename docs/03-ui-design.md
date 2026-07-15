# UI/UX Design Specification

This document details the visual guidelines, design tokens, responsive layout grid, and structural wireframes for the Enterprise Workforce Management System (EWMS) (Sprint 1).

---

## 1. Design Tokens & Styling Tokens

We will use a modern, professional, high-contrast dark theme as the default, with variables set up to support light theme switching.

### Color Palette (Dark Theme)

| Token Name | Hex Code | Visual Sample / Usage |
| :--- | :--- | :--- |
| **Background** | `#0f172a` | Deep blue-gray for the primary app viewport background. |
| **Surface** | `#1e293b` | Slightly lighter blue-gray for cards, forms, tables, and sidebars. |
| **Primary** | `#3b82f6` | Vibrant blue for buttons, active state highlights, and primary links. |
| **Success** | `#10b981` | Emerald green for active statuses and approved actions. |
| **Warning** | `#f59e0b` | Warm amber for pending or probation statuses. |
| **Text Primary** | `#f8fafc` | Off-white for body text, table cells, and headers. |
| **Text Secondary**| `#94a3b8` | Cool gray for subtitles, labels, and muted text. |

### Typography
- **Primary Font**: `Inter, sans-serif` (for tables, inputs, UI chrome).
- **Heading Font**: `Outfit, sans-serif` (for dashboard metrics, page titles).

---

## 2. Layout Structure (Layout Wireframe)

The application uses a persistent split-pane shell:
- **Left Panel (Sidebar)**: Sticky navigation, logo, profile summary.
- **Top Panel (Navbar)**: Page titles, notifications indicator, theme toggle.
- **Main Viewport**: Scrollable workspace area for active features.

```text
+-------------------------------------------------------------------------+
| LOGO  | EWMS | [Icon] Notifications   [Theme Toggle]   [User Avatar]    |
+--------------+----------------------------------------------------------+
|  Dashboard   |                                                          |
|  Employees  *|  Active Viewport: Employees List Directory               |
|  Departments |                                                          |
|  Settings    |  [Search Input...]   [Department Dropdown]  [+ Add Emp]   |
|              |                                                          |
|              |  +----------------------------------------------------+  |
|              |  | Name      | Email      | Department | Status       |  |
|              |  +----------------------------------------------------+  |
|              |  | John Doe  | j.d@co.com | Eng        | [Active]     |  |
|              |  | Jane S.   | j.s@co.com | HR         | [Probation]  |  |
|              |  +----------------------------------------------------+  |
|              |                                                          |
+--------------+----------------------------------------------------------+
```

---

## 3. Wireframes

### 3.1 Dashboard Layout
Aggregates summary statistics and event lists.

```text
+-------------------------------------------------------------------------+
| Dashboard Metrics                                                       |
|                                                                         |
| +-------------------+   +-------------------+   +---------------------+ |
| | Total Employees   |   | Active Leaves     |   | Upcoming Birthdays  | |
| |       142         |   |         3         |   | 2 (Next 30 Days)    | |
| +-------------------+   +-------------------+   +---------------------+ |
|                                                                         |
| Department Distribution (Pie/Bar Chart)                                 |
| +---------------------------------------------------------------------+ |
| | [Bar Chart: ENG: 64, HR: 12, FIN: 8, MKT: 6]                        | |
| +---------------------------------------------------------------------+ |
+-------------------------------------------------------------------------+
```

---

### 3.2 Employee Registration Form
Modal overlay or dedicated screen for registering/editing employees.

```text
+-------------------------------------------------------------------------+
| Register New Employee                                        [X] Close  |
|                                                                         |
|  First Name                 Last Name                                   |
|  [                     ]    [                     ]                     |
|                                                                         |
|  Email Address              Phone Number (Optional)                     |
|  [                     ]    [                     ]                     |
|                                                                         |
|  Job Title                  Department                                  |
|  [                     ]    [ Select Department v ]                     |
|                                                                         |
|  Join Date                  Initial Status                              |
|  [ mm/dd/yyyy          ]    ( ) Active   (*) Probation                  |
|                                                                         |
|  [ Cancel ]                                          [ Save Employee ]  |
+-------------------------------------------------------------------------+
```
- **Validation feedback**: Under each invalid field, a small text string `#ef4444` (red) will render dynamically if requirements (e.g. valid email, required names) are missed.
