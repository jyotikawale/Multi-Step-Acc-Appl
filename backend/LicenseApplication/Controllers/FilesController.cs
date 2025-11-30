using Microsoft.AspNetCore.Mvc;
using LicenseApplication.DTOs;
using LicenseApplication.Services;

namespace LicenseApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly FileStorageService _fileStorageService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(FileStorageService fileStorageService, ILogger<FilesController> logger)
    {
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<FileMetadataDto>> UploadFile([FromForm] Guid applicationId, [FromForm] IFormFile file)
    {
        try
        {
            var result = await _fileStorageService.UploadFileAsync(applicationId, file);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, new { message = "An error occurred while uploading the file" });
        }
    }

    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(Guid fileId)
    {
        try
        {
            var result = await _fileStorageService.DeleteFileAsync(fileId);
            if (!result)
                return NotFound(new { message = $"File with ID {fileId} not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileId}", fileId);
            return StatusCode(500, new { message = "An error occurred while deleting the file" });
        }
    }

    [HttpGet("{fileId}")]
    public async Task<IActionResult> DownloadFile(Guid fileId)
    {
        try
        {
            var result = await _fileStorageService.GetFileAsync(fileId);
            if (result == null)
                return NotFound(new { message = $"File with ID {fileId} not found" });

            var (fileData, contentType, fileName) = result.Value;
            return File(fileData, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileId}", fileId);
            return StatusCode(500, new { message = "An error occurred while downloading the file" });
        }
    }
}
