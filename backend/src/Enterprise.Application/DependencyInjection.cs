using Microsoft.Extensions.DependencyInjection;
using Enterprise.Application.Services;

namespace Enterprise.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        return services;
    }
}
