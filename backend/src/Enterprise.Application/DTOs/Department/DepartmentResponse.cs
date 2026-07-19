using System;

namespace Enterprise.Application.DTOs.Department;

public record DepartmentResponse(
    Guid Id,
    string Name,
    string Code,
    Guid? ManagerId,
    string? ManagerName,
    int EmployeeCount);
