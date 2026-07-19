using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Domain.Entities;

namespace Enterprise.Domain.Repositories;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<(Department Department, string? ManagerName, int EmployeeCount)>> GetAllWithDetailsAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(Department department, CancellationToken cancellationToken = default);

    void Update(Department department);

    void Delete(Department department);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
