using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Repositories;

namespace Enterprise.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly EnterpriseDbContext _context;

    public DepartmentRepository(EnterpriseDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<(Department Department, string? ManagerName, int EmployeeCount)>> GetAllWithDetailsAsync(
        CancellationToken cancellationToken = default)
    {
        var data = await _context.Departments
            .GroupJoin(
                _context.Employees,
                d => d.Id,
                e => e.DepartmentId,
                (d, emps) => new { Department = d, Employees = emps }
            )
            .Select(x => new
            {
                x.Department,
                EmployeeCount = x.Employees.Count(),
                ManagerName = _context.Employees
                    .Where(e => e.Id == x.Department.ManagerId)
                    .Select(e => e.FirstName + " " + e.LastName)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return data.Select(x => (x.Department, x.ManagerName, x.EmployeeCount));
    }

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        await _context.Departments.AddAsync(department, cancellationToken);
    }

    public void Update(Department department)
    {
        _context.Departments.Update(department);
    }

    public void Delete(Department department)
    {
        _context.Departments.Remove(department);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var nameLower = name.Trim().ToLowerInvariant();
        return await _context.Departments.AnyAsync(d => d.Name.ToLower() == nameLower, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var codeLower = code.Trim().ToLowerInvariant();
        return await _context.Departments.AnyAsync(d => d.Code.ToLower() == codeLower, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
