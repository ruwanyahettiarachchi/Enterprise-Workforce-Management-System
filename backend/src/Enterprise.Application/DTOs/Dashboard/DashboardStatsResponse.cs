using System;
using System.Collections.Generic;

namespace Enterprise.Application.DTOs.Dashboard;

public record DepartmentDistributionDto(
    string DepartmentName,
    string Code,
    int EmployeeCount);

public record UpcomingEventDto(
    Guid EmployeeId,
    string EmployeeName,
    string JobTitle,
    string EventType,
    DateTime EventDate);

public record DashboardStatsResponse(
    int TotalActiveEmployees,
    int TotalDepartments,
    int TotalProbationEmployees,
    IEnumerable<DepartmentDistributionDto> DepartmentDistributions,
    IEnumerable<UpcomingEventDto> UpcomingEvents);
