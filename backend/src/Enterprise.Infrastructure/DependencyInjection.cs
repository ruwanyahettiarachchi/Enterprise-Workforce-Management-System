using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Enterprise.Domain.Repositories;
using Enterprise.Infrastructure.Repositories;

namespace Enterprise.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EnterpriseDbContext>(options =>
            options.UseSqlServer(connectionString, b => 
                b.MigrationsAssembly(typeof(EnterpriseDbContext).Assembly.FullName)));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        return services;
    }
}
