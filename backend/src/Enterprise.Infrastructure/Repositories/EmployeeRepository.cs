using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Repositories;

namespace Enterprise.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EnterpriseDbContext _context;

    public EmployeeRepository(EnterpriseDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees.ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Employees.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLowerInvariant();
            query = query.Where(e => 
                e.FirstName.ToLower().Contains(search) || 
                e.LastName.ToLower().Contains(search) || 
                e.Email.ToLower().Contains(search) || 
                e.JobTitle.ToLower().Contains(search));
        }

        if (status.HasValue)
        {
            query = query.Where(e => (int)e.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
    }

    public void Update(Employee employee)
    {
        _context.Employees.Update(employee);
    }

    public void Delete(Employee employee)
    {
        _context.Employees.Remove(employee);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailLower = email.Trim().ToLowerInvariant();
        return await _context.Employees.AnyAsync(e => e.Email == emailLower, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
