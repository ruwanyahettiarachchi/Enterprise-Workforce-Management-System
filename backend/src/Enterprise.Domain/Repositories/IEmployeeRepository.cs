using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Domain.Entities;

namespace Enterprise.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? status,
        Guid? departmentId,
        string? jobTitle,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
        
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    
    void Update(Employee employee);
    
    void Delete(Employee employee);
    
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNICAsync(string nic, CancellationToken cancellationToken = default);

    Task<bool> HasEmployeesInDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
