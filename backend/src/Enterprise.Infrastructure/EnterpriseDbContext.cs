using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Enterprise.Domain.Entities;

namespace Enterprise.Infrastructure;

public class EnterpriseDbContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();

    public EnterpriseDbContext(DbContextOptions<EnterpriseDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
