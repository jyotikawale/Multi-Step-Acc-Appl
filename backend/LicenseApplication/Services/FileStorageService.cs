using Microsoft.EntityFrameworkCore;
using LicenseApplication.Data;
using LicenseApplication.DTOs;
using LicenseApplication.Models;

namespace LicenseApplication.Services;

public class FileStorageService
{
    private readonly string _uploadPath;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FileStorageService> _logger;
    private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB
    private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc" };

    public FileStorageService(string uploadPath, ApplicationDbContext context, ILogger<FileStorageService> logger)
    {
        _uploadPath = uploadPath;
        _context = context;
        _logger = logger;

        // Create upload directory if it doesn't exist
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<FileMetadataDto> UploadFileAsync(Guid applicationId, IFormFile file)
    {
        // Validate application exists
        var application = await _context.Applications.FindAsync(applicationId);
        if (application == null)
            throw new KeyNotFoundException($"Application with ID {applicationId} not found");

        // Validate file
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided");

        if (file.Length > _maxFileSize)
            throw new ArgumentException($"File size exceeds maximum allowed size of {_maxFileSize / (1024 * 1024)}MB");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw new ArgumentException($"File type {extension} is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save metadata to database
        var fileMetadata = new FileMetadata
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            FileName = file.FileName,
            FilePath = uniqueFileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };

        _context.FileMetadata.Add(fileMetadata);
        await _context.SaveChangesAsync();

        _logger.LogInformation("File uploaded: {FileName} for application {ApplicationId}", file.FileName, applicationId);

        return new FileMetadataDto
        {
            Id = fileMetadata.Id,
            FileName = fileMetadata.FileName,
            ContentType = fileMetadata.ContentType,
            FileSize = fileMetadata.FileSize,
            UploadedAt = fileMetadata.UploadedAt
        };
    }

    public async Task<bool> DeleteFileAsync(Guid fileId)
    {
        var fileMetadata = await _context.FileMetadata.FindAsync(fileId);
        if (fileMetadata == null)
            return false;

        // Delete physical file
        var filePath = Path.Combine(_uploadPath, fileMetadata.FilePath);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Delete metadata from database
        _context.FileMetadata.Remove(fileMetadata);
        await _context.SaveChangesAsync();

        _logger.LogInformation("File deleted: {FileId}", fileId);

        return true;
    }

    public async Task<(byte[] fileData, string contentType, string fileName)?> GetFileAsync(Guid fileId)
    {
        var fileMetadata = await _context.FileMetadata.FindAsync(fileId);
        if (fileMetadata == null)
            return null;

        var filePath = Path.Combine(_uploadPath, fileMetadata.FilePath);
        if (!File.Exists(filePath))
            return null;

        var fileData = await File.ReadAllBytesAsync(filePath);
        return (fileData, fileMetadata.ContentType, fileMetadata.FileName);
    }
}
