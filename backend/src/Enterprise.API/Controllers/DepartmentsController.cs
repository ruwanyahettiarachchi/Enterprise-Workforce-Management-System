using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Application.DTOs.Department;
using Enterprise.Application.Services;

namespace Enterprise.API.Controllers;

/// <summary>
/// Handles department creation, lookup, and administration settings.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    /// Retrieves all registered corporate departments.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of department records.</returns>
    /// <response code="200">Successfully retrieved list.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DepartmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);
        return Ok(departments);
    }

    /// <summary>
    /// Resolves a single department's metadata by ID.
    /// </summary>
    /// <param name="id">The unique identifier Guid of the department.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested department details.</returns>
    /// <response code="200">Successfully resolved department.</response>
    /// <response code="404">If no department with the specified ID was found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DepartmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);
        if (department == null)
        {
            return NotFound($"Department with ID '{id}' was not found.");
        }
        return Ok(department);
    }

    /// <summary>
    /// Creates a new department within the organization.
    /// </summary>
    /// <param name="request">The department details (Name, unique code, managerId).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly registered department details.</returns>
    /// <response code="201">Successfully registered department.</response>
    /// <response code="400">If department name or code is already registered, or other inputs fail validations.</response>
    [HttpPost]
    [ProducesResponseType(typeof(DepartmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _departmentService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
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
    /// Updates an existing department's settings or assigned manager.
    /// </summary>
    /// <param name="id">The unique identifier Guid of the department.</param>
    /// <param name="request">The update specifications.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No Content.</returns>
    /// <response code="204">Successfully updated department configuration.</response>
    /// <response code="400">If department name or code conflicts exist, or validation fails.</response>
    /// <response code="404">If the department does not exist.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _departmentService.UpdateAsync(id, request, cancellationToken);
            if (!updated)
            {
                return NotFound($"Department with ID '{id}' was not found.");
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
    /// Removes a department from the system database.
    /// </summary>
    /// <param name="id">The unique identifier Guid of the department.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No Content.</returns>
    /// <response code="204">Successfully deleted department.</response>
    /// <response code="400">If there are active employees assigned to this department.</response>
    /// <response code="404">If the department does not exist.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _departmentService.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound($"Department with ID '{id}' was not found.");
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
