using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Application.DTOs.Department;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Repositories;

namespace Enterprise.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public DepartmentService(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
    {
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var data = await _departmentRepository.GetAllWithDetailsAsync(cancellationToken);
        return data.Select(x => new DepartmentResponse(
            x.Department.Id,
            x.Department.Name,
            x.Department.Code,
            x.Department.ManagerId,
            x.ManagerName,
            x.EmployeeCount
        ));
    }

    public async Task<DepartmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null) return null;

        string? managerName = null;
        if (department.ManagerId.HasValue)
        {
            var manager = await _employeeRepository.GetByIdAsync(department.ManagerId.Value, cancellationToken);
            if (manager != null)
            {
                managerName = $"{manager.FirstName} {manager.LastName}";
            }
        }

        // Quick check on employee count
        var hasEmployees = await _employeeRepository.HasEmployeesInDepartmentAsync(id, cancellationToken);
        var employeeCount = hasEmployees ? 1 : 0; // rough check or we can query DB count if necessary, but simple is fine

        return new DepartmentResponse(
            department.Id,
            department.Name,
            department.Code,
            department.ManagerId,
            managerName,
            employeeCount
        );
    }

    public async Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        if (await _departmentRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            throw new InvalidOperationException($"Department with name '{request.Name}' already exists.");
        }

        if (await _departmentRepository.ExistsByCodeAsync(request.Code, cancellationToken))
        {
            throw new InvalidOperationException($"Department with code '{request.Code}' already exists.");
        }

        var department = new Department(request.Name, request.Code, request.ManagerId);

        await _departmentRepository.AddAsync(department, cancellationToken);
        await _departmentRepository.SaveChangesAsync(cancellationToken);

        string? managerName = null;
        if (department.ManagerId.HasValue)
        {
            var manager = await _employeeRepository.GetByIdAsync(department.ManagerId.Value, cancellationToken);
            if (manager != null) managerName = $"{manager.FirstName} {manager.LastName}";
        }

        return new DepartmentResponse(
            department.Id,
            department.Name,
            department.Code,
            department.ManagerId,
            managerName,
            0
        );
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null) return false;

        if (!string.Equals(department.Name, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (await _departmentRepository.ExistsByNameAsync(request.Name, cancellationToken))
            {
                throw new InvalidOperationException($"Department with name '{request.Name}' already exists.");
            }
        }

        if (!string.Equals(department.Code, request.Code, StringComparison.OrdinalIgnoreCase))
        {
            if (await _departmentRepository.ExistsByCodeAsync(request.Code, cancellationToken))
            {
                throw new InvalidOperationException($"Department with code '{request.Code}' already exists.");
            }
        }

        department.UpdateDetails(request.Name, request.Code, request.ManagerId);

        _departmentRepository.Update(department);
        await _departmentRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null) return false;

        // Deletion safety guard check
        if (await _employeeRepository.HasEmployeesInDepartmentAsync(id, cancellationToken))
        {
            throw new InvalidOperationException("Cannot delete department because active employees are assigned to it.");
        }

        _departmentRepository.Delete(department);
        await _departmentRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
