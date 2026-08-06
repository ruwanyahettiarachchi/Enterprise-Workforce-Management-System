using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Application.DTOs.Employee;
using Enterprise.Application.Services;

namespace Enterprise.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Retrieves a paginated list of employees with advanced search and filters.
    /// </summary>
    /// <param name="searchTerm">Optional query searching first name, last name, email, or job title.</param>
    /// <param name="status">Optional status code filter: 0 = Active, 1 = Probation, 2 = Terminated.</param>
    /// <param name="departmentId">Optional unique identifier Guid of the department filter.</param>
    /// <param name="jobTitle">Optional job title value match filter.</param>
    /// <param name="pageNumber">1-indexed page identifier (default 1).</param>
    /// <param name="pageSize">Amount of records returned per page (default 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of employee responses.</returns>
    /// <response code="200">Successfully retrieved paginated directory.</response>
    /// <response code="400">If pagination offsets are less than 1.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string? searchTerm,
        [FromQuery] int? status,
        [FromQuery] Guid? departmentId,
        [FromQuery] string? jobTitle,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("Page number and page size must be greater than zero.");
        }

        var (items, totalCount) = await _employeeService.GetPagedAsync(
            searchTerm,
            status,
            departmentId,
            jobTitle,
            pageNumber,
            pageSize,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return Ok(new
        {
            items,
            pageNumber,
            pageSize,
            totalPages,
            totalCount
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            return NotFound($"Employee with ID '{id}' was not found.");
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _employeeService.UpdateAsync(id, request, cancellationToken);
            if (!updated)
            {
                return NotFound($"Employee with ID '{id}' was not found.");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _employeeService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound($"Employee with ID '{id}' was not found.");
        }

        return NoContent();
    }
}
