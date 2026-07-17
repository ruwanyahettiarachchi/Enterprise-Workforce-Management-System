export enum EmployeeStatus {
  Active = 0,
  Probation = 1,
  Terminated = 2
}

export interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  jobTitle: string;
  status: EmployeeStatus;
  joinDate: string;
  departmentId?: string;
  departmentName?: string;
}

export interface PaginatedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}
