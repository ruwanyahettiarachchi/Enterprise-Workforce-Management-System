using System;

namespace Enterprise.Application.DTOs.Employee;

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string JobTitle,
    DateTime JoinDate,
    Guid? DepartmentId);
