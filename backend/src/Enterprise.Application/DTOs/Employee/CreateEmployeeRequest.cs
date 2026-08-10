using System;
using Enterprise.Domain.Enums;

namespace Enterprise.Application.DTOs.Employee;

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string JobTitle,
    DateTime JoinDate,
    Guid? DepartmentId,
    string NIC,
    DateTime DateOfBirth,
    Gender Gender,
    MaritalStatus MaritalStatus,
    string AddressLine1,
    string District,
    string City,
    string PostalCode);
