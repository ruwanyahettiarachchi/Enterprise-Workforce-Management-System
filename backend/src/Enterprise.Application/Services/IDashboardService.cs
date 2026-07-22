using System.Threading;
using System.Threading.Tasks;
using Enterprise.Application.DTOs.Dashboard;

namespace Enterprise.Application.Services;

public interface IDashboardService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}
