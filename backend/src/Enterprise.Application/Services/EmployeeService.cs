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

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null) return null;

        return MapToResponse(employee);
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

        var responses = items.Select(MapToResponse);
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

        // 2. Create Rich Domain Model Entity
        var employee = new Employee(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.JobTitle,
            request.JoinDate,
            request.DepartmentId
        );

        // 3. Persist
        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(employee);
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

        // 3. Update details (invokes Domain validation guards)
        employee.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.JobTitle,
            request.DepartmentId
        );

        // 4. Update status
        employee.TransitionStatus(request.Status);

        // 5. Persist
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

    private static EmployeeResponse MapToResponse(Employee employee)
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
            employee.DepartmentId
        );
    }
}
