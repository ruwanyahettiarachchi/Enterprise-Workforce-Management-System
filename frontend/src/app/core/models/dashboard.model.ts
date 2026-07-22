export interface DepartmentDistribution {
  departmentName: string;
  code: string;
  employeeCount: number;
}

export interface UpcomingEvent {
  employeeId: string;
  employeeName: string;
  jobTitle: string;
  eventType: string;
  eventDate: string;
}

export interface DashboardStats {
  totalActiveEmployees: number;
  totalDepartments: number;
  totalProbationEmployees: number;
  departmentDistributions: DepartmentDistribution[];
  upcomingEvents: UpcomingEvent[];
}
