using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Application.DTOs.Employee;
using Enterprise.Application.Services;

namespace Enterprise.API.Controllers;

/// <summary>
/// Handles workforce member registration, directory listings, and demographic profile administration.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Retrieves a paginated list of employees with search and status filters.
    /// </summary>
    /// <param name="searchTerm">Optional query searching first name, last name, email, or job title.</param>
    /// <param name="status">Optional status code filter: 0 = Active, 1 = Probation, 2 = Terminated.</param>
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

    /// <summary>
    /// Fetches a specific employee's comprehensive profile by ID.
    /// </summary>
    /// <param name="id">The unique identifier Guid of the employee.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested employee details.</returns>
    /// <response code="200">Employee profile successfully resolved.</response>
    /// <response code="404">If no employee with the specified ID was found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            return NotFound($"Employee with ID '{id}' was not found.");
        }

        return Ok(employee);
    }

    /// <summary>
    /// Registers a new employee profile including demographic and geographic values.
    /// </summary>
    /// <param name="request">The registration payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created employee response with locations.</returns>
    /// <response code="201">Employee successfully registered.</response>
    /// <response code="400">If email or NIC is already registered, or other inputs fail domain constraints.</response>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Updates an existing employee's editable profile details.
    /// </summary>
    /// <param name="id">The unique identifier Guid of the employee.</param>
    /// <param name="request">The update payload with new address or status details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No Content.</returns>
    /// <response code="204">Profile updated successfully.</response>
    /// <response code="400">If input values are invalid or email conflicts exist.</response>
    /// <response code="404">If the employee profile does not exist.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Removes an employee profile from the system.
    /// </summary>
    /// <param name="id">The unique identifier Guid of the employee.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No Content.</returns>
    /// <response code="204">Employee successfully removed.</response>
    /// <response code="404">If the employee profile does not exist.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
