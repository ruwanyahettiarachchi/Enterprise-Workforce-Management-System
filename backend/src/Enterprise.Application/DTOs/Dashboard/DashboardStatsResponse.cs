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
    DateTime EventDate,
    int? MilestoneYears = null); // e.g. "5 Year Work Anniversary"

public record GenderBreakdownDto(
    int MaleCount,
    int FemaleCount);

public record MaritalStatusBreakdownDto(
    string Status,
    int Count);

public record DashboardStatsResponse(
    int TotalActiveEmployees,
    int TotalDepartments,
    int TotalProbationEmployees,
    int TotalTerminatedEmployees,
    IEnumerable<DepartmentDistributionDto> DepartmentDistributions,
    IEnumerable<UpcomingEventDto> UpcomingEvents,
    GenderBreakdownDto GenderBreakdown,
    IEnumerable<MaritalStatusBreakdownDto> MaritalStatusBreakdowns);
