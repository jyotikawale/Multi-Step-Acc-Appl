using Microsoft.AspNetCore.Mvc;
using LicenseApplication.DTOs;
using LicenseApplication.Services;

namespace LicenseApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationService _applicationService;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(ApplicationService applicationService, ILogger<ApplicationsController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> CreateApplication([FromBody] CreateApplicationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                )
            });
        }

        try
        {
            var result = await _applicationService.CreateApplicationAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application");
            return StatusCode(500, new { message = "An error occurred while creating the application" });
        }
    }

    [HttpPost("draft")]
    public async Task<ActionResult<ApplicationDto>> CreateDraft([FromBody] CreateApplicationDto dto)
    {
        try
        {
            var result = await _applicationService.CreateDraftAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating draft");
            return StatusCode(500, new { message = "An error occurred while creating the draft" });
        }
    }

    [HttpPut("{id}/draft")]
    public async Task<ActionResult<ApplicationDto>> UpdateDraft(Guid id, [FromBody] UpdateDraftDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                )
            });
        }

        try
        {
            var result = await _applicationService.UpdateDraftAsync(id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating draft {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the draft" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationDto>> GetById(Guid id)
    {
        try
        {
            var result = await _applicationService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Application with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application {Id}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the application" });
        }
    }

    [HttpGet("reference/{referenceNumber}")]
    public async Task<ActionResult<ApplicationDto>> GetByReferenceNumber(string referenceNumber)
    {
        try
        {
            var result = await _applicationService.GetByReferenceNumberAsync(referenceNumber);
            if (result == null)
                return NotFound(new { message = $"Application with reference {referenceNumber} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application by reference {Reference}", referenceNumber);
            return StatusCode(500, new { message = "An error occurred while retrieving the application" });
        }
    }

    [HttpPost("validate")]
    public IActionResult ValidateApplication([FromBody] CreateApplicationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                isValid = false,
                errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                )
            });
        }

        return Ok(new { isValid = true });
    }
}
