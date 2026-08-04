using System;
using Enterprise.Domain.Enums;

namespace Enterprise.Application.DTOs.Employee;

public record EmployeeResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string JobTitle,
    EmployeeStatus Status,
    DateTime JoinDate,
    Guid? DepartmentId,
    string? DepartmentName,
    string NIC,
    DateTime DateOfBirth,
    Gender Gender,
    MaritalStatus MaritalStatus,
    string AddressLine1,
    string District,
    string City,
    string PostalCode);
