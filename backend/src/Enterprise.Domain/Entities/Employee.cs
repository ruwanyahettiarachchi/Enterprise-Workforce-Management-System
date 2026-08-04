using System;
using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;

namespace Enterprise.Domain.Entities;

public class Employee : AuditableEntity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public string JobTitle { get; private set; } = null!;
    public EmployeeStatus Status { get; private set; }
    public DateTime JoinDate { get; private set; }
    public Guid? DepartmentId { get; private set; }

    // New demographic & address fields
    public string NIC { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public MaritalStatus MaritalStatus { get; private set; }
    public string AddressLine1 { get; private set; } = null!;
    public string District { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;

    // Required by EF Core
    protected Employee() { }

    public Employee(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string jobTitle,
        DateTime joinDate,
        Guid? departmentId,
        string nic,
        DateTime dateOfBirth,
        Gender gender,
        MaritalStatus maritalStatus,
        string addressLine1,
        string district,
        string city,
        string postalCode,
        EmployeeStatus status = EmployeeStatus.Active)
    {
        if (string.IsNullOrWhiteSpace(nic))
            throw new ArgumentException("NIC is required.", nameof(nic));

        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentException("Address Line 1 is required.", nameof(addressLine1));

        if (string.IsNullOrWhiteSpace(district))
            throw new ArgumentException("District is required.", nameof(district));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal Code is required.", nameof(postalCode));

        NIC = nic.Trim().ToUpperInvariant();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        JoinDate = joinDate;

        UpdateDetails(firstName, lastName, email, phone, jobTitle, departmentId, maritalStatus, addressLine1, district, city, postalCode);
        TransitionStatus(status);
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string jobTitle,
        Guid? departmentId,
        MaritalStatus maritalStatus,
        string addressLine1,
        string district,
        string city,
        string postalCode)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("A valid email address is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new ArgumentException("Job title cannot be empty.", nameof(jobTitle));

        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentException("Address Line 1 cannot be empty.", nameof(addressLine1));

        if (string.IsNullOrWhiteSpace(district))
            throw new ArgumentException("District cannot be empty.", nameof(district));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal Code cannot be empty.", nameof(postalCode));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        JobTitle = jobTitle.Trim();
        DepartmentId = departmentId;
        
        // Editable demographic & address details
        MaritalStatus = maritalStatus;
        AddressLine1 = addressLine1.Trim();
        District = district.Trim();
        City = city.Trim();
        PostalCode = postalCode.Trim();
        
        LastModifiedAt = DateTime.UtcNow;
    }

    public void TransitionStatus(EmployeeStatus newStatus)
    {
        Status = newStatus;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignDepartment(Guid? departmentId)
    {
        DepartmentId = departmentId;
        LastModifiedAt = DateTime.UtcNow;
    }
}
