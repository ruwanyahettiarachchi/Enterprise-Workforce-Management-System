using System;
using System.Text.RegularExpressions;
using Enterprise.Domain.Common;

namespace Enterprise.Domain.Entities;

public class Department : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public Guid? ManagerId { get; private set; }

    // Required by EF Core
    protected Department() { }

    public Department(string name, string code, Guid? managerId)
    {
        UpdateDetails(name, code, managerId);
    }

    public void UpdateDetails(string name, string code, Guid? managerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Department code cannot be empty.", nameof(code));

        var cleanCode = code.Trim().ToUpperInvariant();
        if (cleanCode.Length > 10 || !Regex.IsMatch(cleanCode, "^[A-Z0-9]+$"))
            throw new ArgumentException("Department code must be alphanumeric and up to 10 characters.", nameof(code));

        Name = name.Trim();
        Code = cleanCode;
        ManagerId = managerId;
        
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignManager(Guid? managerId)
    {
        ManagerId = managerId;
        LastModifiedAt = DateTime.UtcNow;
    }
}
