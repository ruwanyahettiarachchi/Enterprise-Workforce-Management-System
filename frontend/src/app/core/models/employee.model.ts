export enum EmployeeStatus {
  Active = 0,
  Probation = 1,
  Terminated = 2
}

export enum Gender {
  Male = 0,
  Female = 1
}

export enum MaritalStatus {
  Single = 0,
  Married = 1,
  Divorced = 2,
  Widowed = 3
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
  nic: string;
  dateOfBirth: string;
  gender: Gender;
  maritalStatus: MaritalStatus;
  addressLine1: string;
  district: string;
  city: string;
  postalCode: string;
}

export interface PaginatedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}
