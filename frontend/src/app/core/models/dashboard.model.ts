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
  milestoneYears?: number;
}

export interface DashboardStats {
  totalActiveEmployees: number;
  totalDepartments: number;
  totalProbationEmployees: number;
  totalTerminatedEmployees: number;
  departmentDistributions: DepartmentDistribution[];
  upcomingEvents: UpcomingEvent[];
  genderBreakdown: {
    maleCount: number;
    femaleCount: number;
  };
  maritalStatusBreakdowns: {
    status: string;
    count: number;
  }[];
}
