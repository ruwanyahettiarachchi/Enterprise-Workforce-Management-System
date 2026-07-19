using System;

namespace Enterprise.Application.DTOs.Department;

public record CreateDepartmentRequest(
    string Name,
    string Code,
    Guid? ManagerId);
