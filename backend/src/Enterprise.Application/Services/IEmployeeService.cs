using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Application.DTOs.Employee;

namespace Enterprise.Application.Services;

public interface IEmployeeService
{
    Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<(IEnumerable<EmployeeResponse> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
        
    Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
