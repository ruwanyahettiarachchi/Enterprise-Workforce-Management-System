using System;
using Enterprise.Domain.Enums;

namespace Enterprise.Application.DTOs.Employee;

public record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string JobTitle,
    Guid? DepartmentId,
    EmployeeStatus Status);
