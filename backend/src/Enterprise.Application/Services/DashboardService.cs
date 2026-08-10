using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Application.DTOs.Dashboard;
using Enterprise.Domain.Enums;
using Enterprise.Domain.Repositories;

namespace Enterprise.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public DashboardService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        // 1. Load employees and department details
        var allEmployees = (await _employeeRepository.GetAllAsync(cancellationToken)).ToList();
        var deptDetails = (await _departmentRepository.GetAllWithDetailsAsync(cancellationToken)).ToList();

        var totalActive = allEmployees.Count(e => e.Status == EmployeeStatus.Active);
        var totalProbation = allEmployees.Count(e => e.Status == EmployeeStatus.Probation);
        var totalTerminated = allEmployees.Count(e => e.Status == EmployeeStatus.Terminated);
        var totalDepts = deptDetails.Count;

        // 2. Department distribution
        var distribution = deptDetails.Select(d => new DepartmentDistributionDto(
            d.Department.Name,
            d.Department.Code,
            d.EmployeeCount
        )).ToList();

        // Include unassigned count if any employees lack a department
        var unassignedCount = allEmployees.Count(e => !e.DepartmentId.HasValue);
        if (unassignedCount > 0)
        {
            distribution.Add(new DepartmentDistributionDto("Unassigned", "N/A", unassignedCount));
        }

        // 3. Calculate upcoming work anniversaries within the next 30 days
        var today = DateTime.UtcNow.Date;
        var upcomingEvents = new List<UpcomingEventDto>();

        foreach (var emp in allEmployees.Where(e => e.Status != EmployeeStatus.Terminated))
        {
            var joinMonth = emp.JoinDate.Month;
            var joinDay = emp.JoinDate.Day;

            // Calculate anniversary date safely handling leap years
            DateTime anniversaryThisYear;
            if (joinMonth == 2 && joinDay == 29 && !DateTime.IsLeapYear(today.Year))
            {
                anniversaryThisYear = new DateTime(today.Year, 2, 28);
            }
            else
            {
                anniversaryThisYear = new DateTime(today.Year, joinMonth, joinDay);
            }

            if (anniversaryThisYear < today)
            {
                if (joinMonth == 2 && joinDay == 29 && !DateTime.IsLeapYear(today.Year + 1))
                {
                    anniversaryThisYear = new DateTime(today.Year + 1, 2, 28);
                }
                else
                {
                    anniversaryThisYear = new DateTime(today.Year + 1, joinMonth, joinDay);
                }
            }

            var daysUntil = (anniversaryThisYear - today).TotalDays;
            if (daysUntil <= 30)
            {
                var years = anniversaryThisYear.Year - emp.JoinDate.Year;
                var eventLabel = years == 0 ? "New Joiner" : $"{years} Year Anniversary";

                upcomingEvents.Add(new UpcomingEventDto(
                    emp.Id,
                    $"{emp.FirstName} {emp.LastName}",
                    emp.JobTitle,
                    eventLabel,
                    anniversaryThisYear,
                    years
                ));
            }
        }

        var sortedEvents = upcomingEvents.OrderBy(e => e.EventDate).Take(6).ToList();

        // 4. Calculate Gender Representation
        var maleCount = allEmployees.Count(e => e.Gender == Gender.Male);
        var femaleCount = allEmployees.Count(e => e.Gender == Gender.Female);
        var genderBreakdown = new GenderBreakdownDto(maleCount, femaleCount);

        // 5. Calculate Marital Status Breakdown
        var maritalBreakdowns = allEmployees
            .GroupBy(e => e.MaritalStatus)
            .Select(g => new MaritalStatusBreakdownDto(g.Key.ToString(), g.Count()))
            .ToList();

        return new DashboardStatsResponse(
            totalActive,
            totalDepts,
            totalProbation,
            totalTerminated,
            distribution,
            sortedEvents,
            genderBreakdown,
            maritalBreakdowns
        );
    }
}
