using System;

namespace Enterprise.Application.DTOs.Department;

public record UpdateDepartmentRequest(
    string Name,
    string Code,
    Guid? ManagerId);
