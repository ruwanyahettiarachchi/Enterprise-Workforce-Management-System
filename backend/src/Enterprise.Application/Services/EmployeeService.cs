using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Application.DTOs.Employee;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Repositories;

namespace Enterprise.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null) return null;

        string? departmentName = null;
        if (employee.DepartmentId.HasValue)
        {
            var dept = await _departmentRepository.GetByIdAsync(employee.DepartmentId.Value, cancellationToken);
            departmentName = dept?.Name;
        }

        return MapToResponse(employee, departmentName);
    }

    public async Task<(IEnumerable<EmployeeResponse> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _employeeRepository.GetPagedAsync(
            searchTerm,
            status,
            pageNumber,
            pageSize,
            cancellationToken);

        // Bulk resolve department names to optimize performance (avoids N+1 query loop)
        var deptIds = items
            .Where(e => e.DepartmentId.HasValue)
            .Select(e => e.DepartmentId!.Value)
            .Distinct()
            .ToList();

        var deptDict = new Dictionary<Guid, string>();
        if (deptIds.Any())
        {
            var depts = await _departmentRepository.GetAllAsync(cancellationToken);
            deptDict = depts
                .Where(d => deptIds.Contains(d.Id))
                .ToDictionary(d => d.Id, d => d.Name);
        }

        var responses = items.Select(e => MapToResponse(
            e,
            e.DepartmentId.HasValue && deptDict.TryGetValue(e.DepartmentId.Value, out var name) ? name : null
        ));

        return (responses, totalCount);
    }

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Email Uniqueness Invariant Check
        var emailLower = request.Email.Trim().ToLowerInvariant();
        if (await _employeeRepository.ExistsByEmailAsync(emailLower, cancellationToken))
        {
            throw new InvalidOperationException($"An employee with email '{request.Email}' already exists.");
        }

        // 2. Validate department if provided
        string? departmentName = null;
        if (request.DepartmentId.HasValue)
        {
            var dept = await _departmentRepository.GetByIdAsync(request.DepartmentId.Value, cancellationToken);
            if (dept == null)
            {
                throw new ArgumentException("The specified department does not exist.", nameof(request.DepartmentId));
            }
            departmentName = dept.Name;
        }

        // 3. Create Rich Domain Model Entity
        var employee = new Employee(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.JobTitle,
            request.JoinDate,
            request.DepartmentId
        );

        // 4. Persist
        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(employee, departmentName);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Fetch existing Entity
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null) return false;

        // 2. Verify Email Uniqueness if changing
        var emailLower = request.Email.Trim().ToLowerInvariant();
        if (!string.Equals(employee.Email, emailLower, StringComparison.OrdinalIgnoreCase))
        {
            if (await _employeeRepository.ExistsByEmailAsync(emailLower, cancellationToken))
            {
                throw new InvalidOperationException($"An employee with email '{request.Email}' already exists.");
            }
        }

        // 3. Validate department if provided
        if (request.DepartmentId.HasValue)
        {
            var dept = await _departmentRepository.GetByIdAsync(request.DepartmentId.Value, cancellationToken);
            if (dept == null)
            {
                throw new ArgumentException("The specified department does not exist.", nameof(request.DepartmentId));
            }
        }

        // 4. Update details (invokes Domain validation guards)
        employee.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.JobTitle,
            request.DepartmentId
        );

        // 5. Update status
        employee.TransitionStatus(request.Status);

        // 6. Persist
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null) return false;

        _employeeRepository.Delete(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static EmployeeResponse MapToResponse(Employee employee, string? departmentName)
    {
        return new EmployeeResponse(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.Phone,
            employee.JobTitle,
            employee.Status,
            employee.JoinDate,
            employee.DepartmentId,
            departmentName
        );
    }
}
