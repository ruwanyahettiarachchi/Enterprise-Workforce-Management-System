using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Application.DTOs.Dashboard;
using Enterprise.Application.Services;

namespace Enterprise.API.Controllers;

/// <summary>
/// Provides workforce dashboard stats, counts, and upcoming corporate work anniversaries.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Computes aggregated metrics, department shares, and upcoming 30-day employee work anniversaries.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dashboard analytics charts data.</returns>
    /// <response code="200">Successfully compiled analytics statistics.</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetDashboardStatsAsync(cancellationToken);
        return Ok(stats);
    }
}
