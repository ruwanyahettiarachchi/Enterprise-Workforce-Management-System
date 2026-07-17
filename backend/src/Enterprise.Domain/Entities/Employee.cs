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

    // Required by EF Core for migrations/restoration
    protected Employee() { }

    public Employee(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string jobTitle,
        DateTime joinDate,
        Guid? departmentId,
        EmployeeStatus status = EmployeeStatus.Active)
    {
        UpdateDetails(firstName, lastName, email, phone, jobTitle, departmentId);
        TransitionStatus(status);
        JoinDate = joinDate;
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string jobTitle,
        Guid? departmentId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("A valid email address is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new ArgumentException("Job title cannot be empty.", nameof(jobTitle));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        JobTitle = jobTitle.Trim();
        DepartmentId = departmentId;
        
        LastModifiedAt = DateTime.UtcNow;
    }

    public void TransitionStatus(EmployeeStatus newStatus)
    {
        // Enforce business rules for transition if needed
        // E.g., once Terminated, transitioning back to Active might require verification
        Status = newStatus;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignDepartment(Guid? departmentId)
    {
        DepartmentId = departmentId;
        LastModifiedAt = DateTime.UtcNow;
    }
}
